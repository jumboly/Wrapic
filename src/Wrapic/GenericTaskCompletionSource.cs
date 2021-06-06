using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Wrapic
{
    internal sealed class GenericTaskCompletionSource
    {
        private static readonly ConcurrentDictionary<Type, Accessor> _accessors = new();
        
        private readonly Accessor _accessor;
        private readonly object _instance;

        public GenericTaskCompletionSource(Type resultType)
        {
            _accessor = _accessors.GetOrAdd(resultType, t => new Accessor(t));
            _instance = _accessor.CreateInstance();
        }

        public void SetResult(object result)
        {
            _accessor.SetResult(_instance, result);
        }

        public void SetException(Exception exception)
        {
            _accessor.SetException(_instance, exception);
        }

        public Task Task => _accessor.GetTask(_instance);
        
        private class Accessor
        {
            private readonly Type _type;
            private readonly MethodInfo _setResultMethod;
            private readonly MethodInfo _setExceptionMethod;
            private readonly PropertyInfo _taskProperty;

            public Accessor(Type resultType)
            {
                _type = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
                _setResultMethod = GetMethod(nameof(TaskCompletionSource.SetResult), resultType);
                _setExceptionMethod = GetMethod(nameof(TaskCompletionSource.SetException), typeof(Exception));
                _taskProperty = GetProperty(nameof(TaskCompletionSource.Task));
            }

            public object CreateInstance()
            {
                return Activator.CreateInstance(_type) ?? throw new Exception($"インスタンスが作成できません: {nameof(TaskCompletionSource)}<{_type.Name}>");
            }

            public void SetResult(object instance, object result)
            {
                _setResultMethod.Invoke(instance, new[] {result});
            }

            public void SetException(object instance, Exception exception)
            {
                _setExceptionMethod.Invoke(instance, new object?[] {exception});
            }

            public Task GetTask(object instance)
            {
                return _taskProperty.GetValue(instance) as Task ?? throw new Exception($"{nameof(Task)} が取得できません");
            }

            private MethodInfo GetMethod(string name, params Type[] types)
            {
                return _type.GetMethod(name, types) ?? throw new Exception($"メソッドが取得できません: {name}");
            }

            private PropertyInfo GetProperty(string name)
            {
                return _type.GetProperty(name) ?? throw new Exception($"プロパティが取得できません: {name}");
            }
        }
    }
}