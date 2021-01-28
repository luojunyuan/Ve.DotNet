using MeCab;
using System.Collections.Generic;

namespace Ve.DotNet
{
    public class VcWord
    {
        // These five seem underdeveloped and underpopulated:
        private string reading;
        private string transcription;
        private Grammar grammar;
        private string lemma; // "聞く"
        private Pos part_of_speech; // eg. Pos.Noun
        private List<MeCabNode> tokens = new (); // those which were eaten up by this one word: {聞か, せ, られ}
        private string word; // "聞かせられ"

        public VcWord(
            string read,
            string pronunciation,
            Grammar grammar,
            string basic,
            Pos part_of_speech,
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

        public Pos PartOfSpeech 
        {
            get => part_of_speech;
            set => this.part_of_speech = value;
        }

        public string Lemma { get => lemma; }

        public List<MeCabNode> GetTokens { get => tokens; }

        public string Word { get => word; }

        public void AppendToWord(string suffix)
        {
            word += suffix;
        }

        public void AppendToReading(string suffix) 
        {
            reading += suffix;
        }

        public void AppendToTranscription(string suffix) 
        {
            transcription += suffix;
        }

        // Not sure when this would change.
        public void AppendToLemma(string suffix) 
        {
            lemma += suffix;
        }

        public override string ToString() 
        {
            return word;
        }
    }

    public enum Pos
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
