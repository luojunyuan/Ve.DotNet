using MeCab;
using MeCab.Extension.IpaDic;
using System;

namespace VeDotNetConsoleSample
{
    class Program
    {
        private const string Example1 = "太郎はこの本を笑也を見た女性に渡した。";

        static void Main(string[] args)
        {
            // Using Mecab.Extention.IpaDic namespace
            var tagger = MeCabTagger.Create();

            foreach (var word in tagger.ParseToNodes(Example1).ParseVeWords())
            {
                Console.WriteLine(
                    $"{word.Word.PadRight(4, '　')} {word.Pronunciation.PadRight(5, '　')} " +
                    $"{word.PartOfSpeech} {word.Lemma}");
            }
        }
    }
}
