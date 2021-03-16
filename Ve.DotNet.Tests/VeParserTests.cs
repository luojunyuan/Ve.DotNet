using MeCab;
using MeCab.Extension.IpaDic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ve.DotNet.Tests
{
    [TestClass()]
    public class VeParserTests
    {
        private const string OccurredNullSample = "なのに、たしかに、俺はここでは腫れ物の様な存在であった。";

        [TestMethod()]
        public void VeParseWordsBugFixOne()
        {
            const string? testString = OccurredNullSample;

            var tagger = MeCabTagger.Create();
            foreach (var _ in tagger.ParseToNodes(testString).ParseVeWords())
            { }
        }
    }
}