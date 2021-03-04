using System;
using System.Diagnostics;
using MeCab;
using MeCab.Extension.IpaDic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ve.DotNet.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string Example1 = "稼ぎます";
        private const string Example2 = "すもももももももものうち。";
        private const string Example3 = "行く川のながれは絶えずして、しかももとの水にあらず。";
        private const string Example4 = "アルバイトとして来てくれた庭坂莉良さん、汐ノ宮璃貝さん――。";
        
        [TestMethod]
        public void VeWordBasicOutputTestMethod()
        {
            var sentence = Example3;
            
            var tagger = MeCabTagger.Create();
            var enumerableSet = tagger.ParseToNodes(sentence);
            
            var wordList = VeParser.Words(enumerableSet);
            foreach (var word in wordList)
            {
                Trace.WriteLine(
                    $"{word.Word.PadRight(5, '　')} {word.Pronunciation} " +
                    $"{word.PartOfSpeech.ToString()} {word.Lemma}");
            }
        }
        
        [TestMethod]
        public void MeCabIpaDicNodeMethodExample()
        {
            var sentence = Example1;
            
            var tagger = MeCabTagger.Create();
            var enumerableSet = tagger.ParseToNodes(sentence);

            using var enumerator = enumerableSet.GetEnumerator();
            enumerator.MoveNext(); // Feature: BOS/EOS,*,*,*,*,*,*,*,*
            enumerator.MoveNext();
            MeCabNode node = enumerator.Current!;
            Trace.WriteLine($"Alpha: {node.Alpha}");
            Trace.WriteLine($"Beta: {node.Beta}");
            Trace.WriteLine($"`MeCabNode` BNext: {node.BNext}");
            Trace.WriteLine($"BPos: {node.BPos}");
            Trace.WriteLine($"CharType: {node.CharType}");
            Trace.WriteLine($"Cost: {node.Cost}");
            Trace.WriteLine($"`MeCabNode` ENext: {node.ENext}");
            Trace.WriteLine($"EPos: {node.EPos}");
            Trace.WriteLine($"Feature: {node.Feature}");
            Trace.WriteLine($"GetConjugatedForm() Feature[4]: {node.GetConjugatedForm()}"); // CTYPE
            Trace.WriteLine($"GetInflection() Feature[5]: {node.GetInflection()}");         // CFORM
            Trace.WriteLine($"GetOriginalForm() Feature[6]: {node.GetOriginalForm()}");     // BASIC
            Trace.WriteLine($"GetPartsOfSpeech() Feature[0]: {node.GetPartsOfSpeech()}");
            Trace.WriteLine($"GetPartsOfSpeechSection1() Feature[1]: {node.GetPartsOfSpeechSection1()}");
            Trace.WriteLine($"GetPartsOfSpeechSection2() Feature[2]: {node.GetPartsOfSpeechSection2()}");
            Trace.WriteLine($"GetPartsOfSpeechSection3() Feature[3]: {node.GetPartsOfSpeechSection3()}");
            Trace.WriteLine($"GetPronunciation() Feature[8]: {node.GetPronounciation()}"); 
            Trace.WriteLine($"GetReading() Feature[7]: {node.GetReading()}");
            Trace.WriteLine($"IsBest: {node.IsBest}");
            Trace.WriteLine($"LCAttr: {node.LCAttr}");
            Trace.WriteLine($"Length: {node.Length}");
            Trace.WriteLine($"`MeCabNode` Next: {node.Next}");
            Trace.WriteLine($"PosId: {node.PosId}");
            Trace.WriteLine($"`MeCabNode` Prev: {node.Prev}");
            Trace.WriteLine($"Prob: {node.Prob}");
            Trace.WriteLine($"RCAttr: {node.RCAttr}");
            Trace.WriteLine($"RLength: {node.RLength}");
            Trace.WriteLine($"Stat: {node.Stat}");
            Trace.WriteLine($"Surface: {node.Surface}");
            Trace.WriteLine($"WCost: {node.WCost}");
        }

        [TestMethod]
        public void VeWordParserDebug()
        {
            var tagger = MeCabTagger.Create();
            string sentence = Example4;
            foreach (var veWord in tagger.ParseToNodes(sentence).ParseVeWords())
            {
                Trace.WriteLine(veWord.Word);
            }
        }
    }
}
