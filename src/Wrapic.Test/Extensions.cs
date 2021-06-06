using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Wrapic.Test
{
    public static class Extensions
    {
        public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder builder, IEnumerable<(string, string)> items)
        {
            return builder.AddInMemoryCollection(items.Select(it => new KeyValuePair<string, string>(it.Item1, it.Item2)));
        }
        public static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder builder, params (string, string)[] items)
        {
            return AddInMemoryCollection(builder, (IEnumerable<(string, string)>) items);
        }
    }
}