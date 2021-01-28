# Ve.DotNet

This repository is a port of [Ve](https://github.com/Kimtaro/ve) (a warpper of mecab or kuromoji, similar to cabocha)

based on [MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet)

```csharp
class Program
{
    static void Main(string[] args)
    {
        string sentence = "アルバイトとして来てくれた庭坂莉良さん、汐ノ宮璃貝さん――。";
        var tagger = MeCabTagger.Create();
        IEnumerable<MeCabNode> enumbrableSet = tagger.ParseToNodes(sentence);
        var veParser = new VeParse(enumbrableSet);
        
        foreach (var word in veParser.Words())
        {
            Console.WriteLine(word);
        }
    }
}
```

Result
```cmd
アルバイトとして来てくれた庭坂莉良さん、汐ノ宮璃貝さん――。
アルバイト
として
来てくれた
庭坂
莉良
さん
、
汐ノ宮
璃貝
さん
――
。
```
