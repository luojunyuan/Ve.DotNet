using MeCab;
using System.Collections.Generic;
using Ve.DotNet;

// ReSharper disable once CheckNamespace
namespace MeCab.Extension.IpaDic
{
    public static class VeParser
    {
        public static IEnumerable<VeWord> ParseVeWords(this IEnumerable<MeCabNode> nodeEnumerable)
        {
            return Ve.DotNet.VeParser.Words(nodeEnumerable);
        }
    }
}
