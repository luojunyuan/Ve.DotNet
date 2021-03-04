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

using System;
using System.Collections.Generic;
using MeCab;
using MeCab.Extension.IpaDic;

namespace Ve.DotNet
{
    public static class VeParser
    {
        private const string NoData = "*";

        /// <summary>
        /// The Generator
        /// </summary>
        /// <param name="nodeEnumerable">Include IpaDic info</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IEnumerable<VeWord> Words(IEnumerable<MeCabNode> nodeEnumerable)
        {
            var tokenArray = new List<MeCabNode>(nodeEnumerable).ToArray()[1..^1];
            
            List<VeWord> wordList = new();
            MeCabNode previous = new();

            // FIXME: Not stable
            for (var i = 0; i < tokenArray.Length; i++)
            {
                var finalSlot = wordList.Count - 1;
                MeCabNode current = tokenArray[i];
                MeCabNode following;
                PartOfSpeech partOfSpeech;
                var grammar = Grammar.Unassigned;

                var eatNext = false;
                var eatLemma = true;
                var attachToPrevious = false;
                var alsoAttachToLemma = false;
                var updatePos = false;

                switch (current.GetPartsOfSpeech())
                {
                    case "名詞":
                        {
                            partOfSpeech = PartOfSpeech.名詞;
                            if (current.GetPartsOfSpeechSection1().Equals(NoData))
                                break;

                            switch (current.GetPartsOfSpeechSection1())
                            {
                                case "固有名詞":
                                    partOfSpeech = PartOfSpeech.固有名詞;
                                    break;
                                case "代名詞":
                                    partOfSpeech = PartOfSpeech.代名詞;
                                    break;
                                case "副詞可能":
                                case "サ変接続":
                                case "形容動詞語幹":
                                case "ナイ形容詞語幹":
                                    // Refers to line 213 of Ve.
                                    if (current.GetPartsOfSpeechSection2().Equals(NoData))
                                        break;

                                    // protects against array overshooting.
                                    if (i == tokenArray.Length - 1)
                                        break;

                                    following = tokenArray[i + 1];
                                    switch (following.GetConjugatedForm()) // [CTYPE]
                                    {
                                        case "サ変・スル":
                                            partOfSpeech = PartOfSpeech.動詞;
                                            eatNext = true;
                                            break;
                                        case "特殊・ダ":
                                            partOfSpeech = PartOfSpeech.形容詞;
                                            if (following.GetPartsOfSpeechSection1().Equals("体言接続"))
                                            {
                                                eatNext = true;
                                                eatLemma = false;
                                            }
                                            break;
                                        case "特殊・ナイ":
                                            partOfSpeech = PartOfSpeech.形容詞;
                                            eatNext = true;
                                            break;
                                        default:
                                            if (following.GetPartsOfSpeech().Equals("助詞")
                                                && following.Surface.Equals("に"))
                                            {
                                                // Ve script redundantly (I think) also has eat_next = false here.
                                                partOfSpeech = PartOfSpeech.副詞;
                                            }
                                            break;
                                    }
                                    break;
                                case "非自立":
                                case "特殊":
                                    // Refers to line 233 of Ve.
                                    if (current.GetPartsOfSpeechSection2().Equals(NoData))
                                        break;

                                    // protects against array overshooting.
                                    if (i == tokenArray.Length - 1)
                                        break;

                                    following = tokenArray[i + 1];

                                    switch (current.GetPartsOfSpeechSection2())
                                    {
                                        case "副詞可能":
                                            if (following.GetPartsOfSpeech().Equals("助詞")
                                                && following.Surface.Equals("に"))
                                            {
                                                partOfSpeech = PartOfSpeech.副詞;
                                 
                                                
                                                // Changed this to false because 'case JOSHI' has 'attach_to_previous = true'.
                                                eatNext = false;
                                            }
                                            break;
                                        case "助動詞語幹":
                                            if (following.GetConjugatedForm().Equals("特殊・ダ"))
                                            {
                                                partOfSpeech = PartOfSpeech.動詞;
                                                grammar = Grammar.Auxiliary;
                                                if (following.GetInflection().Equals("体言接続"))
                                                {
                                                    eatNext = true;
                                                }
                                            }
                                            else if (following.GetPartsOfSpeech().Equals("助詞")
                                                && following.GetPartsOfSpeechSection2().Equals("副詞化"))
                                            {
                                                partOfSpeech = PartOfSpeech.副詞;
                                                eatNext = true;
                                            }
                                            break;
                                        case "形容動詞語幹":
                                            partOfSpeech = PartOfSpeech.形容詞;
                                            if (following.GetConjugatedForm().Equals("特殊・ダ") &&
                                                following.GetInflection().Equals("体言接続")
                                                || following.GetPartsOfSpeechSection1().Equals("連体化"))
                                            {
                                                eatNext = true;
                                            }
                                            break;
                                    }
                                    break;
                                case "数":
                                    // TODO: "recurse and find following numbers and add to this word. Except non-numbers 
                                    // like 幾"
                                    // Refers to line 261.
                                    partOfSpeech = PartOfSpeech.数;
                                    if (wordList.Count > 0 && wordList[finalSlot].PartOfSpeech.Equals(PartOfSpeech.数))
                                    {
                                        attachToPrevious = true;
                                        alsoAttachToLemma = true;
                                    }
                                    break;
                                case "接尾":
                                    // Refers to line 267.
                                    switch (current.GetPartsOfSpeechSection2())
                                    {
                                        case "人名":
                                            partOfSpeech = PartOfSpeech.人名接尾;
                                            break;
                                        case "特殊":
                                            if (current.GetOriginalForm().Equals("さ"))
                                            {
                                                updatePos = true;
                                                partOfSpeech = PartOfSpeech.名詞;
                                            }
                                            attachToPrevious = true;
                                            break;
                                        default: // 助数词
                                            alsoAttachToLemma = true;
                                            attachToPrevious = true;
                                            break;
                                    }
                                    break;
                                case "接続詞的":
                                    partOfSpeech = PartOfSpeech.接続詞;
                                    break;
                                case "動詞非自立的":
                                    partOfSpeech = PartOfSpeech.動詞;
                                    grammar = Grammar.Nominal; // not using.
                                    break;
                            }
                        }   
                        break;
                    case "接頭詞":
                        // TODO: "elaborate this when we have the "main part" feature for words?"
                        partOfSpeech = PartOfSpeech.接頭詞;
                        break;
                    case "助動詞":
                        // Refers to line 290.
                        partOfSpeech = PartOfSpeech.助詞;
                        List<string> tokushuList = new() { "特殊・タ", "特殊・ナイ", "特殊・タイ", "特殊・マス", "特殊・ヌ" };
                        if (!previous.GetPartsOfSpeechSection1().Equals("係助詞")
                            && tokushuList.Contains(current.GetConjugatedForm()))
                        {
                            attachToPrevious = true;
                        }
                        else if (current.GetConjugatedForm().Equals("不変化型")
                            && current.GetOriginalForm().Equals("ん"))
                        {
                            attachToPrevious = true;
                        }
                        else if (current.GetConjugatedForm().Equals("特殊・ダ") || current.GetConjugatedForm().Equals("特殊・デス")
                            && !current.Surface.Equals("な"))
                        {
                            partOfSpeech = PartOfSpeech.動詞;
                        }
                        break;
                    case "動詞":
                        // Refers to line 299.
                        partOfSpeech = PartOfSpeech.動詞;
                        switch (current.GetPartsOfSpeechSection1())
                        {
                            case "接尾":
                                attachToPrevious = true;
                                break;
                            case "非自立":
                                if (!current.GetInflection().Equals("命令ｉ"))
                                {
                                    attachToPrevious = true;
                                }
                                break;
                        }
                        break;
                    case "形容詞":
                        partOfSpeech = PartOfSpeech.形容詞;
                        break;
                    case "助詞":
                        // Refers to line 309.
                        partOfSpeech = PartOfSpeech.助詞;
                        List<string> qualifyingList2 = new() { "て", "で", "ば" };
                        switch (current.GetPartsOfSpeechSection1())
                        {
                            case "接続助詞":
                                if (qualifyingList2.Contains(current.Surface) ||
                                    current.Surface.Equals("に"))
                                {
                                    attachToPrevious = true;
                                }
                                break;
                            case "連体化": // の 
                            case "係助詞": // は
                            case "格助詞": // で
                            case "終助詞": // な
                                break;
                        }
                        break;
                    case "連体詞":
                        partOfSpeech = PartOfSpeech.連体詞;
                        break;
                    case "接続詞":
                        partOfSpeech = PartOfSpeech.接続詞;
                        break;
                    case "副詞":
                        partOfSpeech = PartOfSpeech.副詞;
                        break;
                    case "記号":
                        partOfSpeech = PartOfSpeech.記号;
                        break;
                    case "フィラー":
                    case "感動詞":
                        partOfSpeech = PartOfSpeech.感動詞;
                        break;
                    case "その他":
                        partOfSpeech = PartOfSpeech.その他;
                        break;
                    default:
                        partOfSpeech = PartOfSpeech.未定だ;
                        break;
                        // C'est une catastrophe
                }

                if (attachToPrevious && wordList.Count > 0)
                {
                    // these sometimes try to add to null readings.
                    wordList[finalSlot].Tokens.Add(current);
                    wordList[finalSlot].AppendToWord(current.Surface);
                    wordList[finalSlot].AppendToReading(current.GetReading());
                    wordList[finalSlot].AppendToTranscription(current.GetPronounciation());
                    if (alsoAttachToLemma)
                    {
                        wordList[finalSlot].AppendToLemma(current.GetOriginalForm()); // lemma == basic.
                    }
                    if (updatePos)
                    {
                        wordList[finalSlot].UpdatePartOfSpeech(partOfSpeech);
                    }
                }
                else
                {
                    VeWord word = new(
                        current.GetPronounciation(),
                        current.GetReading(),
                        current.GetOriginalForm(),
                        partOfSpeech,
                        grammar,
                        current.Surface,
                        current
                    );

                    if (eatNext)
                    {
                        if (i == tokenArray.Length - 1)
                        {
                            throw new InvalidOperationException("There's a path that allows array overshooting.");
                        }
                        following = tokenArray[i + 1];
                        word.Tokens.Add(following);
                        word.AppendToWord(following.Surface);
                        word.AppendToReading(following.GetReading());
                        word.AppendToTranscription(following.GetPronounciation());
                        if (eatLemma)
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
    }
}
