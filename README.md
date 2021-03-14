# Ve.DotNet

Channel | Status
-|-
CI | [![CI](https://github.com/luojunyuan/Ve.DotNet/workflows/CI/badge.svg)](https://github.com/luojunyuan/Ve.DotNet/actions)
NuGet.org | [![NuGet.org](https://img.shields.io/nuget/v/luojunyuan.Ve.DotNet.svg)](https://www.nuget.org/packages/luojunyuan.Ve.DotNet/)

This repository is a port of [Ve](https://github.com/Kimtaro/ve) (a warpper of mecab or kuromoji, similar to cabocha)

based on [MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet)

```csharp
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
```

Result
```cmd
太郎　　 タロー　　 固有名詞 太郎
は　　　 ワ　　　　 助詞 は
この　　 コノ　　　 連体詞 この
本　　　 ホン　　　 名詞 本
を　　　 ヲ　　　　 助詞 を
笑也　　 エミヤ　　 固有名詞 笑也
を　　　 ヲ　　　　 助詞 を
見た　　 ミタ　　　 動詞 見る
女性　　 ジョセイ　 名詞 女性
に　　　 ニ　　　　 助詞 に
渡した　 ワタシタ　 動詞 渡す
。　　　 。　　　　 記号 。
```
