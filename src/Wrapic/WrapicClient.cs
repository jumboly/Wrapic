using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wrapic
{
    public class WrapicClient : DispatchProxy
    {
        public static T Create<T>(HttpClient httpClient, string baseUrl, IWrapicSerializer serializer)
        {
            var proxy = DispatchProxy.Create<T, WrapicClient>();
            
            Debug.Assert(proxy is WrapicClient);
            (proxy as WrapicClient).Init(httpClient, baseUrl, serializer);
            
            return proxy;
        }

        private HttpClient _httpClient;
        private string _baseUrl;
        private IWrapicSerializer _serializer;

        private void Init(HttpClient httpClient, string baseUrl, IWrapicSerializer serializer)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _serializer = serializer;
        }
        
        protected override object Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            var action = targetMethod?.Name ?? throw new ArgumentException(nameof(targetMethod));
            var interfaceName = targetMethod.DeclaringType?.Name ?? throw new ArgumentException("インターフェース名が取得できません", nameof(targetMethod));
            
            var match = Regex.Match(interfaceName, "I(.+)Api$");
            if (!match.Success)
            {
                throw new Exception($"インターフェースは 'I{{コントローラ名}}Api' である必要があります: {interfaceName}");
            }

            var controllerName = match.Groups[1].Value;

            var uri = new Uri(string.Join("/", _baseUrl.TrimEnd('/'), controllerName, action));

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Headers =
                {
                    Accept = {_serializer.MediaType}
                }
            };

            if (args.Length > 0)
            {
                var arg = args[0];
                if (arg != null)
                {
                    var bytes = _serializer.Serialize(arg, targetMethod.GetParameters()[0].ParameterType);
                    request.Content = new ByteArrayContent(bytes)
                    {
                        Headers = {ContentType = _serializer.MediaType}
                    };
                }
            }

            var httpTask = _httpClient.SendAsync(request);

            var resultType = targetMethod.ReturnType.GetGenericArguments().FirstOrDefault();
            if (resultType == null)
            {
                // 戻り値が Task
                return VoidHandle(httpTask);
            }
            else
            {
                // 戻り値が Task<T>
                return ReturnHandle(httpTask, resultType);
            }
        }
        
        private Task VoidHandle(Task<HttpResponseMessage> task)
        {
            var tcs = new TaskCompletionSource();

            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    tcs.SetException(t.Exception);
                }
                else
                {
                    try
                    {
                        t.Result.EnsureSuccessStatusCode();
                        tcs.SetResult();
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                }
            });

            return tcs.Task;
        }

        private Task ReturnHandle(Task<HttpResponseMessage> task, Type resultType)
        {
            var tcs = new GenericTaskCompletionSource(resultType);

            task.ContinueWith(async t =>
            {
                if (t.Exception != null)
                {
                    tcs.SetException(t.Exception);
                }
                else
                {
                    try
                    {
                        t.Result.EnsureSuccessStatusCode();

                        var responseBytes = await t.Result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        if (responseBytes.Length == 0)
                        {
                            tcs.SetResult(null);
                        }
                        else
                        {
                            var response = _serializer.Deserialize(responseBytes, resultType);
                            tcs.SetResult(response);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                }
            });

            return tcs.Task;
        }
    }
}