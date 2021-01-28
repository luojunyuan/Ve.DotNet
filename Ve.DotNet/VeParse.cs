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
using MeCab.Extension.IpaDic;
using System.Collections.Generic;

namespace Ve.DotNet
{
    class VeParse
    {
        private readonly MeCabNode[] tokenArray;
        private const string NO_DATA = "*";

        public VeParse(IEnumerable<MeCabNode> nodeEnumerable)
        {
            tokenArray = new List<MeCabNode>(nodeEnumerable).ToArray();
        }

        public List<VeWord> Words()
        {
            List<VeWord> wordList = new();
            MeCabNode previous = new();

            for (int i = 0; i < tokenArray.Length; i++)
            {
                int finalSlot = wordList.Count - 1;
                MeCabNode current = tokenArray[i];
                MeCabNode following;
                PartOfSpeech partOfSpeech;
                Grammar grammar = Grammar.Unassigned;
                bool eat_next = false,
                     eat_lemma = true,
                     attach_to_previous = false,
                     also_attach_to_lemma = false,
                     update_pos = false;

                switch (current.GetPartsOfSpeech())
                {
                    case MEISHI:
                        partOfSpeech = PartOfSpeech.Noun;
                        if (current.GetPartsOfSpeechSection1().Equals(NO_DATA))
                            break;

                        switch (current.GetPartsOfSpeechSection1())
                        {
                            case KOYUUMEISHI:
                                partOfSpeech = PartOfSpeech.ProperNoun;
                                break;
                            case DAIMEISHI:
                                partOfSpeech = PartOfSpeech.Pronoun;
                                break;
                            case FUKUSHIKANOU:
                            case SAHENSETSUZOKU:
                            case KEIYOUDOUSHIGOKAN:
                            case NAIKEIYOUSHIGOKAN:
                                // Refers to line 213 of Ve.
                                if (current.GetPartsOfSpeechSection2().Equals(NO_DATA))
                                    break;

                                // protects against array overshooting.
                                if (i == tokenArray.Length - 1)
                                    break;

                                following = tokenArray[i + 1];
                                switch (following.GetConjugatedForm()) // [  CTYPE]
                                {
                                    case SAHEN_SURU:
                                        partOfSpeech = PartOfSpeech.Verb;
                                        eat_next = true;
                                        break;
                                    case TOKUSHU_DA:
                                        partOfSpeech = PartOfSpeech.Adjective;
                                        if (following.GetPartsOfSpeechSection1().Equals(TAIGENSETSUZOKU))
                                        {
                                            eat_next = true;
                                            eat_lemma = false;
                                        }
                                        break;
                                    case TOKUSHU_NAI:
                                        partOfSpeech = PartOfSpeech.Adjective;
                                        eat_next = true;
                                        break;
                                    default:
                                        if (following.GetPartsOfSpeech().Equals(JOSHI)
                                            && following.Surface.Equals(NI)) // getSurfaceForm()
                                        {
                                            // Ve script redundantly (I think) also has eat_next = false here.
                                            partOfSpeech = PartOfSpeech.Adverb;
                                        }
                                        break;
                                }
                                break;
                            case HIJIRITSU:
                            case TOKUSHU:
                                // Refers to line 233 of Ve.
                                if (current.GetPartsOfSpeechSection2().Equals(NO_DATA))
                                    break;

                                // protects against array overshooting.
                                if (i == tokenArray.Length - 1)
                                    break;

                                following = tokenArray[i + 1];

                                switch (current.GetPartsOfSpeechSection2())
                                {
                                    case FUKUSHIKANOU:
                                        if (following.GetPartsOfSpeech().Equals(JOSHI)
                                            && following.Surface.Equals(NI))
                                        {
                                            partOfSpeech = PartOfSpeech.Adverb;
                                            // Changed this to false because 'case JOSHI' has 'attach_to_previous = true'.
                                            eat_next = false;
                                        }
                                        break;
                                    case JODOUSHIGOKAN:
                                        if (following.GetConjugatedForm().Equals(TOKUSHU_DA))
                                        {
                                            partOfSpeech = PartOfSpeech.Verb;
                                            grammar = Grammar.Auxiliary;
                                            if (following.GetInflection().Equals(TAIGENSETSUZOKU))
                                            {
                                                eat_next = true;
                                            }
                                        }
                                        else if (following.GetPartsOfSpeech().Equals(JOSHI)
                                            && following.GetPartsOfSpeechSection2().Equals(FUKUSHIKA))
                                        {
                                            partOfSpeech = PartOfSpeech.Adverb;
                                            eat_next = true;
                                        }
                                        break;
                                    case KEIYOUDOUSHIGOKAN:
                                        partOfSpeech = PartOfSpeech.Adjective;
                                        if (following.GetInflection().Equals(TOKUSHU_DA) &&
                                            following.GetConjugatedForm().Equals(TAIGENSETSUZOKU)
                                            || following.GetPartsOfSpeechSection1().Equals(RENTAIKA))
                                        {
                                            eat_next = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case KAZU:
                                // TODO: "recurse and find following numbers and add to this word. Except non-numbers 
                                // like 幾"
                                // Refers to line 261.
                                partOfSpeech = PartOfSpeech.Number;
                                if (wordList.Count > 0 && wordList[finalSlot].PartOfSpeech.Equals(PartOfSpeech.Number))
                                {
                                    attach_to_previous = true;
                                    also_attach_to_lemma = true;
                                }
                                break;
                            case SETSUBI:
                                // Refers to line 267.
                                if (current.GetPartsOfSpeechSection2().Equals(JINMEI))
                                {
                                    partOfSpeech = PartOfSpeech.Suffix;
                                }
                                else
                                {
                                    if (current.GetPartsOfSpeechSection2().Equals(TOKUSHU) &&
                                        current.GetOriginalForm().Equals(SA))
                                    {
                                        update_pos = true;
                                        partOfSpeech = PartOfSpeech.Noun;
                                    }
                                    else
                                    {
                                        also_attach_to_lemma = true;
                                    }
                                    attach_to_previous = true;
                                }
                                break;
                            case SETSUZOKUSHITEKI:
                                partOfSpeech = PartOfSpeech.Conjunction;
                                break;
                            case DOUSHIHIJIRITSUTEKI:
                                partOfSpeech = PartOfSpeech.Verb;
                                grammar = Grammar.Nominal; // not using.
                                break;
                            default:
                                // Keep partOfSpeech as Noun, as it currently is.
                                break;
                        }
                        break;
                    case SETTOUSHI:
                        // TODO: "elaborate this when we have the "main part" feature for words?"
                        partOfSpeech = PartOfSpeech.Prefix;
                        break;
                    case JODOUSHI:
                        // Refers to line 290.
                        partOfSpeech = PartOfSpeech.Postposition;
                        List<string> qualifyingList1 = new List<string> { TOKUSHU_TA, TOKUSHU_NAI, TOKUSHU_TAI, TOKUSHU_MASU, TOKUSHU_NU };
                        if (!previous.GetPartsOfSpeechSection1().Equals(KAKARIJOSHI)
                                && qualifyingList1.Contains(current.GetConjugatedForm()))
                        {
                            attach_to_previous = true;
                        }
                        else if (current.GetConjugatedForm().Equals(FUHENKAGATA) &&
                            current.GetOriginalForm().Equals(NN))
                        {
                            attach_to_previous = true;
                        }
                        else if (current.GetConjugatedForm().Equals(TOKUSHU_DA) || current.GetConjugatedForm().Equals(TOKUSHU_DESU)
                                && !current.Surface.Equals(NA))
                        {
                            partOfSpeech = PartOfSpeech.Verb;
                        }
                        break;
                    case DOUSHI:
                        // Refers to line 299.
                        partOfSpeech = PartOfSpeech.Verb;
                        switch (current.GetPartsOfSpeechSection1())
                        {
                            case SETSUBI:
                                attach_to_previous = true;
                                break;
                            case HIJIRITSU:
                                if (!current.GetInflection().Equals(MEIREI_I))
                                {
                                    attach_to_previous = true;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case KEIYOUSHI:
                        partOfSpeech = PartOfSpeech.Adjective;
                        break;
                    case JOSHI:
                        // Refers to line 309.
                        partOfSpeech = PartOfSpeech.Postposition;
                        List<string> qualifyingList2 = new List<string> { TE, DE, BA }; // added NI
                        if (current.GetPartsOfSpeechSection1().Equals(SETSUZOKUJOSHI) &&
                            qualifyingList2.Contains(current.Surface)
                            || current.Surface.Equals(NI))
                        {
                            attach_to_previous = true;
                        }
                        break;
                    case RENTAISHI:
                        partOfSpeech = PartOfSpeech.Determiner;
                        break;
                    case SETSUZOKUSHI:
                        partOfSpeech = PartOfSpeech.Conjunction;
                        break;
                    case FUKUSHI:
                        partOfSpeech = PartOfSpeech.Adverb;
                        break;
                    case KIGOU:
                        partOfSpeech = PartOfSpeech.Symbol;
                        break;
                    case FIRAA:
                    case KANDOUSHI:
                        partOfSpeech = PartOfSpeech.Interjection;
                        break;
                    case SONOTA:
                        partOfSpeech = PartOfSpeech.Other;
                        break;
                    default:
                        partOfSpeech = PartOfSpeech.TBD;
                        break;
                        // C'est une catastrophe
                }

                if (attach_to_previous && wordList.Count > 0)
                {
                    // these sometimes try to add to null readings.
                    wordList[finalSlot].Tokens.Add(current);
                    wordList[finalSlot].AppendToWord(current.Surface);
                    wordList[finalSlot].AppendToReading(current.GetReading());
                    wordList[finalSlot].AppendToTranscription(current.GetPronounciation());
                    if (also_attach_to_lemma)
                    {
                        wordList[finalSlot].AppendToLemma(current.GetOriginalForm()); // lemma == basic.
                    }
                    if (update_pos)
                    {
                        wordList[finalSlot].PartOfSpeech = partOfSpeech;
                    }
                }
                else
                {
                    VeWord word = new VeWord(
                        current.GetReading(),
                        current.GetPronounciation(),
                        grammar,
                        current.GetOriginalForm(),
                        partOfSpeech,
                        current.Surface,
                        current
                    );

                    if (eat_next)
                    {
                        if (i == tokenArray.Length - 1)
                        {
                            throw new System.InvalidOperationException("There's a path that allows array overshooting.");
                        }
                        following = tokenArray[i + 1];
                        word.Tokens.Add(following);
                        word.AppendToWord(following.Surface);
                        word.AppendToReading(following.GetReading());
                        word.AppendToTranscription(following.GetPronounciation());
                        if (eat_lemma)
                        {
                            word.AppendToLemma(following.GetOriginalForm());
                        }
                    }
                    wordList.Add(word);
                }
                previous = current;
            }

            return wordList;
        }
        
        // POS1
        private const string MEISHI = "名詞";
        private const string KOYUUMEISHI = "固有名詞";
        private const string DAIMEISHI = "代名詞";
        private const string JODOUSHI = "助動詞";
        private const string KAZU = "数";
        private const string JOSHI = "助詞";
        private const string SETTOUSHI = "接頭詞";
        private const string DOUSHI = "動詞";
        private const string KIGOU = "記号";
        private const string FIRAA = "フィラー";
        private const string SONOTA = "その他";
        private const string KANDOUSHI = "感動詞";
        private const string RENTAISHI = "連体詞";
        private const string SETSUZOKUSHI = "接続詞";
        private const string FUKUSHI = "副詞";
        private const string SETSUZOKUJOSHI = "接続助詞";
        private const string KEIYOUSHI = "形容詞";
        private const string MICHIGO = "未知語";

        // POS2_BLACKLIST and inflection types
        private const string HIJIRITSU = "非自立";
        private const string FUKUSHIKANOU = "副詞可能";
        private const string SAHENSETSUZOKU = "サ変接続";
        private const string KEIYOUDOUSHIGOKAN = "形容動詞語幹";
        private const string NAIKEIYOUSHIGOKAN = "ナイ形容詞語幹";
        private const string JODOUSHIGOKAN = "助動詞語幹";
        private const string FUKUSHIKA = "副詞化";
        private const string TAIGENSETSUZOKU = "体言接続";
        private const string RENTAIKA = "連体化";
        private const string TOKUSHU = "特殊";
        private const string SETSUBI = "接尾";
        private const string SETSUZOKUSHITEKI = "接続詞的";
        private const string DOUSHIHIJIRITSUTEKI = "動詞非自立的";
        private const string SAHEN_SURU = "サ変・スル";
        private const string TOKUSHU_TA = "特殊・タ";
        private const string TOKUSHU_NAI = "特殊・ナイ";
        private const string TOKUSHU_TAI = "特殊・タイ";
        private const string TOKUSHU_DESU = "特殊・デス";
        private const string TOKUSHU_DA = "特殊・ダ";
        private const string TOKUSHU_MASU = "特殊・マス";
        private const string TOKUSHU_NU = "特殊・ヌ";
        private const string FUHENKAGATA = "不変化型";
        private const string JINMEI = "人名";
        private const string MEIREI_I = "命令ｉ";
        private const string KAKARIJOSHI = "係助詞";
        private const string KAKUJOSHI = "格助詞";

        // etc
        private const string NA = "な";
        private const string NI = "に";
        private const string TE = "て";
        private const string DE = "で";
        private const string BA = "ば";
        private const string NN = "ん";
        private const string SA = "さ";
    }
}
