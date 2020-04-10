using System;
using System.Collections.Generic;
using lab3;

namespace parserClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Parser test = new Parser("http://news.baidu.com");
            Parser test = new Parser("http://m.news.cctv.com/2020/04/08/ARTIbjzDJnaeM8IuoYU5QRWH200408.shtml");
            test.CleanUpText();
            Console.WriteLine("Here goes text:");
            Console.Write(test.CleanText);


            test.Tokenize();
            Console.WriteLine("All tokens");
            foreach (KeyValuePair<tokenWord, int> kvp in test.DictOfTokens)
            {
                Console.WriteLine("lemma = {0,8}" + "\t" + "tag = {1}" + "\t" + "occurrences = {2}",
                    kvp.Key.Word, kvp.Key.PartOfSpeech, kvp.Value);
            }


            test.FindImportantWords(5);
            Console.WriteLine("Only important words:");
            foreach (KeyValuePair<tokenWord, float> kvp in test.ListOfimportantWord)
            {
                Console.WriteLine(" importance = {0:0.0000}" + "\t" + "lemma = {1}" + "\t" + "tag = {2}",
                    kvp.Value, kvp.Key.Word, kvp.Key.PartOfSpeech);
            }
        }
    }
}

/*
Chinese:
AD (adverb)
CC (coordinating conj.)
CD (cardinal number)
DT (determiner)
FW (foreign words)
JJ (other noun modifier)
LC (localizer/postposit)
M (measure word)
NN (common noun)
NR (proper noun)
NT (temporal noun)
OD (ordinal number)
P (preposition)
PN (pronoun)
PU (punctuation)
SP(sentence final particle)
VA(predicative adjective)
VV (other verbs)
 
English:
CC Coordinating conjunction
CD Cardinal number
DT Determiner
EX Existential there
FW Foreign word
IN Preposition or subordinating conjunction
JJ Adjective
JJR Adjective, comparative
JJS Adjective, superlative
LS List item marker
MD Modal
NN Noun, singular or mass
NNS Noun, plural
NNP Proper noun, singular
NNPS Proper noun, plural
PDT Predeterminer
POS Possessive ending
PRP Personal pronoun
PRP$ Possessive pronoun
RB Adverb
RBR Adverb, comparative
RBS Adverb, superlative
RP Particle
SYM Symbol
TO to
UH Interjection
VB Verb, base form
VBD Verb, past tense
VBG Verb, gerund or present participle
VBN Verb, past participle
VBP Verb, non­3rd person singular present
VBZ Verb, 3rd person singular present
WDT Wh­determiner
WP Wh­pronoun
WP$ Possessive wh­pronoun
WRB Wh­adverb
*/