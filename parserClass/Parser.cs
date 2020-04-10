using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;
using HtmlAgilityPack;
using ikvm.@internal;
using java.io;
using java.util;
using Console = System.Console;

namespace lab3
{
    public class tokenWord
    {
        private string word;
        private string partOfSpeech;

        public tokenWord(string word, string partOfSpeech)
        {
            this.word = word;
            this.partOfSpeech = partOfSpeech;
        }

        protected bool Equals(tokenWord other)
        {
            return word == other.word && partOfSpeech == other.partOfSpeech;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((tokenWord) obj);
        }

        public string Word => word;

        public string PartOfSpeech => partOfSpeech;

        public override int GetHashCode()
        {
            unchecked
            {
                return ((word != null ? word.GetHashCode() : 0) * 397) ^
                       (partOfSpeech != null ? partOfSpeech.GetHashCode() : 0);
            }
        }
    }

    public class Parser
    {
        private String _cleanText;
        private HtmlWeb _web;
        private HtmlDocument _doc;
        private Dictionary<tokenWord, int> _dictOfTokens; // word and number of occurrences
        private List<KeyValuePair<tokenWord, float>> _listOfimportantWord;

        private String pathToModel = "stanford-corenlp-3.9.2-models";
        
        public string CleanText => _cleanText;
        public List<KeyValuePair<tokenWord, float>> ListOfimportantWord => _listOfimportantWord;
        public Dictionary<tokenWord, int> DictOfTokens => _dictOfTokens;

        public Parser()
        {
            // this._url = "";
            _web = new HtmlWeb();
            _web.OverrideEncoding = Encoding.UTF8;
            _doc = new HtmlDocument();
            _cleanText = "";
            _dictOfTokens = new Dictionary<tokenWord, int>();
        }

        public Parser(String url)
        {
            var html = @url;
            _web = new HtmlWeb();
            _web.OverrideEncoding = Encoding.UTF8;
            _doc = _web.Load(html);
            _cleanText = "";
            _dictOfTokens = new Dictionary<tokenWord, int>();
        }

        public Parser(String url, Encoding encoding)
        {
            var html = @url;
            _web = new HtmlWeb();
            _web.OverrideEncoding = encoding;
            _doc = _web.Load(html);
            _cleanText = "";
            _dictOfTokens = new Dictionary<tokenWord, int>();
        }

        private void ParseUrl()
        {
            if (_doc != null)
            {
                // var words = _doc.DocumentNode?.SelectNodes("//body//text()")?.Select(x => x.InnerText);
                // _cleanText = words != null ? string.Join(" ", words) : String.Empty;
                _cleanText = _doc.DocumentNode.SelectSingleNode("//body").InnerText;
            }
        }

        public void CleanUpText(bool noLines = true, string replacement = "。")
        {
            // cleanText should be empty
            if (_cleanText == "")
            {
                ParseUrl();
            }

            if (noLines)
                _cleanText = Regex.Replace(_cleanText, @"\s+", replacement).Trim();
        }

        public void Tokenize()
        {
            // getting current dit
            var curDir = Environment.CurrentDirectory;
            // make sure your models are located next to the .exe (in the same folder)
            pathToModel = Path.Combine(curDir, pathToModel);

            // Annotation pipeline configuration
            var propsFile = Path.Combine(pathToModel, "StanfordCoreNLP-chinese.properties");
            
            Annotation document = new Annotation(_cleanText);
            // Setup Chinese Properties by loading them from classpath resources
            Properties props = new Properties();
            Console.WriteLine("loading props...");
            props.load(new FileReader(propsFile));

            Directory.SetCurrentDirectory(pathToModel);
            Console.WriteLine("loading core...");
            StanfordCoreNLP corenlp = new StanfordCoreNLP(props);

            Console.WriteLine("annotating...");
            corenlp.annotate(document);


            // Console.WriteLine("Testing part:");


            var sentences = (List) document.get(ClassLiteral<CoreAnnotations.SentencesAnnotation>.Value);

            if (sentences != null)
            {
                int index1 = 0;
                for (int index2 = sentences.size(); index1 < index2; ++index1)
                {
                    CoreMap sentence = (CoreMap) sentences.get(index1);

                    List tokens = (List) sentence.get(ClassLiteral<CoreAnnotations.TokensAnnotation>.Value);

                    Iterator terator1 = tokens.iterator();
                    while (terator1.hasNext())
                    {
                        CoreLabel coreLabel = (CoreLabel) terator1.next();
                        var temp = new tokenWord(coreLabel.lemma(), coreLabel.tag());
                        if (_dictOfTokens.ContainsKey(temp))
                            _dictOfTokens[temp]++;
                        else
                        {
                            _dictOfTokens.Add(temp, 1);
                        }
                    }
                }
            }
            else
            {
                Console.Write("Sentences are null");
            }
        }

        public void FindImportantWords(int threshold = 1)
        {
            Dictionary<tokenWord, float> importantNouns = new Dictionary<tokenWord, float>();
            int numOfNouns = 0;
            foreach (KeyValuePair<tokenWord, int> pair in _dictOfTokens)
            {
                // checking only nouns
                if (pair.Key.PartOfSpeech[0] == 'N')
                {
                    numOfNouns++;
                }
            }

            foreach (KeyValuePair<tokenWord, int> pair in _dictOfTokens)
            {
                // checking only nouns
                if (pair.Key.PartOfSpeech[0] == 'N' && pair.Value > threshold && !importantNouns.ContainsKey(pair.Key))
                {
                    importantNouns.Add(pair.Key, (float) pair.Value / numOfNouns);
                }
            }

            // sorting dict
            var items = from pair in importantNouns
                orderby pair.Value descending
                select pair;

            // saving to local list
            _listOfimportantWord = items.ToList();
        }
    }
}