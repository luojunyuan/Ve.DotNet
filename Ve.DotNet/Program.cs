using System;
using System.Collections.Generic;
using MeCab;
using MeCab.Extension.IpaDic;

namespace Ve.DotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            string sentence = "太郎はこの本を笑也を見た女性に渡した。すもももももももものうち。";
            var tagger = MeCabTagger.Create();
            IEnumerable<MeCabNode> enumbrableSet = tagger.ParseToNodes(sentence);
            var veParser = new VeParse(enumbrableSet);
            var wordList = veParser.Words();
            foreach (var word in wordList)
            {
                Console.WriteLine(word);
            }
            // MeCabNodeUseExample(enumbrableSet);

            // foreach(var cur in tagger.ParseToNodes(sentence))
            // {
            //     Console.WriteLine($"{cur.Surface.PadRight(5)} {cur.Feature}");
            // }
        }

        static void MeCabNodeUseExample(IEnumerable<MeCabNode> enumerableSet)
        {
            var enumerator = enumerableSet.GetEnumerator();
            enumerator.MoveNext(); //Feature: BOS/EOS,*,*,*,*,*,*,*,*
            enumerator.MoveNext();
            MeCabNode node = enumerator.Current;
            Console.WriteLine($"Alpha: {node.Alpha}");
            Console.WriteLine($"Beta: {node.Beta}");
            Console.WriteLine($"`MeCabNode` BNext: {node.BNext}");
            Console.WriteLine($"BPos: {node.BPos}");
            Console.WriteLine($"CharType: {node.CharType}");
            Console.WriteLine($"Cost: {node.Cost}");
            Console.WriteLine($"`MeCabNode` ENext: {node.ENext}");
            Console.WriteLine($"EPos: {node.EPos}");
            Console.WriteLine($"Feature: {node.Feature}");
            Console.WriteLine($"GetConjugatedForm() Feature[4]: {node.GetConjugatedForm()}"); // CTYPE
            Console.WriteLine($"GetInflection() Feature[5]: {node.GetInflection()}");         // CFORM
            Console.WriteLine($"GetOriginalForm() Feature[6]: {node.GetOriginalForm()}");     // BASIC
            Console.WriteLine($"GetPartsOfSpeech() Feature[0]: {node.GetPartsOfSpeech()}");
            Console.WriteLine($"GetPartsOfSpeechSection1() Feature[1]: {node.GetPartsOfSpeechSection1()}");
            Console.WriteLine($"GetPartsOfSpeechSection2() Feature[2]: {node.GetPartsOfSpeechSection2()}");
            Console.WriteLine($"GetPartsOfSpeechSection3() Feature[3]: {node.GetPartsOfSpeechSection3()}");
            Console.WriteLine($"GetPronounciation() Feature[8]: {node.GetPronounciation()}"); 
            Console.WriteLine($"GetReading() Feature[7]: {node.GetReading()}");
            Console.WriteLine($"IsBest: {node.IsBest}");
            Console.WriteLine($"LCAttr: {node.LCAttr}");
            Console.WriteLine($"Length: {node.Length}");
            Console.WriteLine($"`MeCabNode` Next: {node.Next}");
            Console.WriteLine($"PosId: {node.PosId}");
            Console.WriteLine($"`MeCabNode` Prev: {node.Prev}");
            Console.WriteLine($"Prob: {node.Prob}");
            Console.WriteLine($"RCAttr: {node.RCAttr}");
            Console.WriteLine($"RLength: {node.RLength}");
            Console.WriteLine($"Stat: {node.Stat}");
            Console.WriteLine($"Surface: {node.Surface}");
            Console.WriteLine($"WCost: {node.WCost}");
        }
    }
}
