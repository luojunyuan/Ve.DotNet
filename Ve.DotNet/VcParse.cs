using MeCab;
using MeCab.Extension.IpaDic;
using System.Collections.Generic;

namespace Ve.DotNet
{
    class VcParse
    {
        private MeCabNode[] tokenArray;
        private const string NO_DATA = "*";

        public Parse(IEnumerable<MeCabNode> nodeEnumerable)
        {
            tokenArray = new List<MeCabNode>(nodeEnumerable).ToArray();
        }

        public List<MeCabNode> Words()
        {
            List<MeCabNode> wordList = new();
            MeCabNode current = new(), 
                      previous = new(), 
                      following = new();

            for(int i = 0; i < tokenArray.Length; i++){
            {
                int finalSlot = wordList.Count - 1;
                current = tokenArray[i];

                Pos pos = Pos.TBD;
                Grammar grammar = Grammar.Unassigned;
                bool eat_next = false,
                     eat_lemma = true,
                     attach_to_previous = false,
                     also_attach_to_lemma = false,
                     update_pos = false;

                switch(tokenArray[i].GetPartsOfSpeech())
                {
                    case MEISHI:
                    {
                        pos = Pos.Noun;
                        if(tokenArray[i].GetPartsOfSpeechSection1().Equals(NO_DATA)) 
                            break;
                        
                        switch(tokenArray[i].GetPartsOfSpeechSection1())
                        {
                            case KOYUUMEISHI:
                                pos = Pos.ProperNoun;
                                break;
                            case DAIMEISHI:
                                pos = Pos.Pronoun;
                                break;
                            case FUKUSHIKANOU:
                            case SAHENSETSUZOKU:
                            case KEIYOUDOUSHIGOKAN:
                            case NAIKEIYOUSHIGOKAN:
                            {
                                // Refers to line 213 of Ve.
                                if(tokenArray[i].GetPartsOfSpeechSection2().Equals(NO_DATA))
                                    break;
                                    
                                // protects against array overshooting.
                                if(i == tokenArray.Length - 1)
                                    break;

                                following = tokenArray[i + 1];
                                switch(following.GetConjugatedForm()) // [  CTYPE]
                                {
                                    case SAHEN_SURU:
                                        pos = Pos.Verb;
                                        eat_next = true;
                                        break;
                                    case TOKUSHU_DA:
                                        pos = Pos.Adjective;
                                        if(following.GetPartsOfSpeechSection1().Equals(TAIGENSETSUZOKU))
                                        {
                                            eat_next = true;
                                            eat_lemma = false;
                                        }
                                        break;
                                    case TOKUSHU_NAI:
                                        pos = Pos.Adjective;
                                        eat_next = true;
                                        break;
                                    default:
                                        if(following.GetPartsOfSpeech().Equals(JOSHI)
                                                && following.Surface.Equals(NI)) // getSurfaceForm()
                                        {
                                            pos = Pos.Adverb; // Ve script redundantly (I think) also has eat_next = false here.
                                        }
                                        break;
                            }
                                break;
                            }
                            case HIJIRITSU:
                            case TOKUSHU:
                                // Refers to line 233 of Ve.
                                if(tokenArray[i].GetPartsOfSpeechSection2().Equals(NO_DATA)) 
                                    break;

                                // protects against array overshooting.
                                if(i == tokenArray.Length - 1) 
                                    break; 

                                following = tokenArray[i + 1];

                                switch(tokenArray[i].GetPartsOfSpeechSection2()){
                                    case FUKUSHIKANOU:
                                        if(following.GetPartsOfSpeech().Equals(JOSHI)
                                            && following.Surface.Equals(NI))
                                        {
                                            pos = Pos.Adverb;
                                            eat_next = false; // Changed this to false because 'case JOSHI' has 'attach_to_previous = true'.
                                        }
                                        break;
                                    case JODOUSHIGOKAN:
                                        if(following.GetConjugatedForm().Equals(TOKUSHU_DA))
                                        {
                                            pos = Pos.Verb;
                                            grammar = Grammar.Auxiliary;
                                            if(following.GetInflection().Equals(TAIGENSETSUZOKU))
                                            {
                                                eat_next = true;
                                            }
                                        }
                                        else if (following.GetPartsOfSpeech().Equals(JOSHI)
                                            && following.GetPartsOfSpeechSection2().Equals(FUKUSHIKA))
                                        {
                                            pos = Pos.Adverb;
                                            eat_next = true;
                                        }
                                        break;
                                    case KEIYOUDOUSHIGOKAN:
                                        pos = Pos.Adjective;
                                        if(following.GetConjugatedForm().Equals(TOKUSHU_DA) && following.GetConjugatedForm().Equals(TAIGENSETSUZOKU) // confuse
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
                                // TODO: "recurse and find following numbers and add to this word. Except non-numbers like 幾"
                                // Refers to line 261.
                                pos = Pos.Number;
                                if(wordList.Count > 0 && wordList[finalSlot].getPart_of_speech().equals(Pos.Number)){
                                    attach_to_previous = true;
                                    also_attach_to_lemma = true;
                                }
                                break;
                            case SETSUBI:
                                // Refers to line 267.
                                if(currentPOSArray[POS3].equals(JINMEI)) pos = Pos.Suffix;
                                else{
                                    if(currentPOSArray[POS3].equals(TOKUSHU) && current.getAllFeaturesArray()[BASIC].equals(SA)){
                                        update_pos = true;
                                        pos = Pos.Noun;
                                    }
                                    else also_attach_to_lemma = true;
                                    attach_to_previous = true;
                                }
                                break;
                            case SETSUZOKUSHITEKI:
                                pos = Pos.Conjunction;
                                break;
                            case DOUSHIHIJIRITSUTEKI:
                                pos = Pos.Verb;
                                grammar = Grammar.Nominal; // not using.
                                break;
                            default:
                                // Keep Pos as Noun, as it currently is.
                                break;
                        }
                        break;
                    }
                }
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
