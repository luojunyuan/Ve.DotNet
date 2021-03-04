/*
  * This file contains code ported from Kim Ahlström's Ve (MIT License).
  * https://github.com/Kimtaro/ve
  *
  * What follows is the text of the MIT License under which Kim Ahlström's
  * original code was published:
  *
  * The MIT License (MIT)
  * Copyright © 2015 Kim Ahlström <kim.ahlstrom@gmail.com>
  *
  * Permission is hereby granted, free of charge, to any person obtaining a copy
  * of this software and associated documentation files (the “Software”), to deal
  * in the Software without restriction, including without limitation the rights
  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  * copies of the Software, and to permit persons to whom the Software is
  * furnished to do so, subject to the following conditions:
  *
  * The above copyright notice and this permission notice shall be included in
  * all copies or substantial portions of the Software.
  *
  * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  * THE SOFTWARE.
  */
using MeCab;
using System.Collections.Generic;

namespace Ve.DotNet
{
    public class VeWord
    {
        // XXX: Seems not use currently
        private Grammar _grammar;

        public VeWord(
            string pronunciation,
            string reading,
            string lemma,
            PartOfSpeech partOfSpeech,
            Grammar grammar,
            string nodeStr,
            MeCabNode token)
        {
            Pronunciation = pronunciation;
            Reading = reading;
            Lemma = lemma;
            PartOfSpeech = partOfSpeech;

            _grammar = grammar;            
            Word = nodeStr;
            Tokens.Add(token);
        }
        
        // These five properties seem underdeveloped and underpopulated:
        /// <summary>
        /// <para>発音</para>
        /// <para>ep. は->ワ</para>
        /// </summary>
        public string Pronunciation { get; private set; }

        /// <summary>
        /// 読み
        /// </summary>
        public string Reading { get; private set; }

        /// <summary>
        /// <para>活用形、単語のルート</para>
        /// <para>ep. "聞く"</para>
        /// </summary>
        public string Lemma { get; private set; }

        /// <summary>
        /// 品詞
        /// </summary>
        public PartOfSpeech PartOfSpeech { get; private set; }

        /// <summary>
        /// <para>形態素、the surface</para>
        /// <para>The group made by Ve</para>
        /// <para>ep. "聞かせられ"</para>
        /// </summary>
        public string Word { get; private set; }

        /// <summary>
        /// <para>Those which were eaten up by this one word: {聞か, せ, られ}</para>
        /// <para>元のMeCabNode infoが含まれている</para>>
        /// </summary>
        public List<MeCabNode> Tokens { get; } = new();
        
        public void AppendToWord(string suffix) => Word += suffix;
        
        public void AppendToReading(string suffix) => Reading += suffix;
        
        public void AppendToTranscription(string suffix) => Pronunciation += suffix;
        
        // Not sure when this would change.
        public void AppendToLemma(string suffix) => Lemma += suffix;

        public void UpdatePartOfSpeech(PartOfSpeech value) => PartOfSpeech = value;
        
        public override string ToString() => Word;
    }

    public enum PartOfSpeech
    {
        名詞,
        固有名詞,
        代名詞,
        形容詞,
        副詞,
        連体詞,
        助詞,
        動詞,
        人名接尾,
        接頭詞,
        接続詞,
        感動詞,
        数, 
        記号,
        その他,
        未定だ
    }

    public enum Grammar 
    {
        Unassigned,
        
        /// <summary>
        /// Mark as "名詞-動詞非自立的", and not use
        /// </summary>
        Auxiliary,
        
        /// <summary>
        /// Mark as "名詞-特殊-助動詞語幹"
        /// </summary>
        Nominal
    }
}
