using System;
using System.Net.Http.Headers;

namespace Wrapic
{
    public interface IWrapicSerializer
    {
        MediaTypeWithQualityHeaderValue MediaType { get; }
        byte[] Serialize(object value, Type type);
        object Deserialize(ReadOnlySpan<byte> bytes, Type type);
    }
}