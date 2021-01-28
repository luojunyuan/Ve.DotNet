/**
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
  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
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
        // These five seem underdeveloped and underpopulated:
        private string reading;
        private string transcription;
        private Grammar grammar;
        private string lemma; // "聞く"
        private PartOfSpeech part_of_speech; // eg. Pos.Noun
        private List<MeCabNode> tokens = new (); // those which were eaten up by this one word: {聞か, せ, られ}
        private string word; // "聞かせられ"

        public VeWord(
            string read,
            string pronunciation,
            Grammar grammar,
            string basic,
            PartOfSpeech part_of_speech,
            string nodeStr,
            MeCabNode token) 
        {
            this.reading = read;
            this.transcription = pronunciation;
            this.grammar = grammar;
            this.lemma = basic;
            this.part_of_speech = part_of_speech;
            this.word = nodeStr;
            tokens.Add(token);
        }

        public PartOfSpeech PartOfSpeech 
        {
            get => part_of_speech;
            set => this.part_of_speech = value;
        }

        public string Lemma { get => lemma; }

        public List<MeCabNode> Tokens { get => tokens; }

        public string Word { get => word; }

        public void AppendToWord(string suffix) => word += suffix;

        public void AppendToReading(string suffix) => reading += suffix;

        public void AppendToTranscription(string suffix) => transcription += suffix;

        // Not sure when this would change.
        public void AppendToLemma(string suffix) => lemma += suffix;

        public override string ToString() => word;
    }

    public enum PartOfSpeech
    {
        Noun,
        ProperNoun,
        Pronoun,
        Adjective,
        Adverb,
        Determiner,
        Preposition,
        Postposition,
        Verb,
        Suffix,
        Prefix,
        Conjunction,
        Interjection,
        Number,
        Unknown,
        Symbol,
        Other,
        TBD
    }

    public enum Grammar 
    {
        Auxiliary,
        Nominal,
        Unassigned
    }
}
