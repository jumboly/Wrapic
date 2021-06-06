using System;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Wrapic
{
    public sealed class WrapicJsonSerializer : IWrapicSerializer
    {
        private readonly JsonSerializerOptions _options;

        public WrapicJsonSerializer(JsonSerializerOptions? options = default)
        {
            _options = options ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }
        
        public MediaTypeWithQualityHeaderValue MediaType { get; } = MediaTypeWithQualityHeaderValue.Parse("application/json");

        public byte[] Serialize(object value, Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, type, _options);
        }

        public object Deserialize(ReadOnlySpan<byte> bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes, type, _options);
        }
    }
}