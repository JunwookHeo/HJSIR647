using HJSIR647.Set;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Syn.WordNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Support;

namespace HJSIR647.IRSearch
{
    class AdvancedSearchManager : SearchManager
    {
        Similarity similarity;
        WordNetEngine wordNet;
        PorterStemmerAlgorithm.PorterStemmer porterStemmer;

        const string WORDNETDB_PATH = @"..\..\WNdb-3\dict";
        const string BOOST = @"^2";

        protected static string URL_FN = SettingsViewModel.URL;

        protected static string[] FIELDS = { URL_FN, TEXT_FN };

        public AdvancedSearchManager() : base()
        {
            InitLucene();
        }
        private void InitLucene()
        {
            //analyzer = new Lucene.Net.Analysis.WhitespaceAnalyzer();
            //analyzer = new Lucene.Net.Analysis.SimpleAnalyzer(); // Activity 5
            //analyzer = new Lucene.Net.Analysis.StopAnalyzer(); // Activity 5
            //analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30); // Activity 5
            //analyzer = new Lucene.Net.Analysis.Snowball.SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "English"); // Activity 7

            ISet<string> StopWords = new HashSet<string>();

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(@"../../StopWords.txt"))
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        StopWords.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            analyzer = new HjsStandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30, StopWords);

            //parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TEXT_FN, analyzer);
            //parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, FIELDS, analyzer, BOOSTING);
            parser = CreateQueryParser();

            similarity = new HjsSimilarity();

            porterStemmer = new PorterStemmerAlgorithm.PorterStemmer();
            // WordNet Load
            LoadWordNet();
        }

        override protected void CreateSearcher()
        {
            searcher = new IndexSearcher(luceneIndexDirectory);
            searcher.Similarity = similarity;
        }
        private QueryParser CreateQueryParser()
        {
            IDictionary<String, float> boosting = new Dictionary<String, float>();
            if(SettingsViewModel.Instance.SelectedFieldBoosting == SettingsViewModel.URL)
            {
                boosting.Add(URL_FN, 2);
                boosting.Add(TEXT_FN, 1);
            }
            else if (SettingsViewModel.Instance.SelectedFieldBoosting == SettingsViewModel.TEXT)
            {
                boosting.Add(URL_FN, 1);
                boosting.Add(TEXT_FN, 2);
            }
            else
            {
                boosting.Add(URL_FN, 1);
                boosting.Add(TEXT_FN, 1);
            }

            return new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, FIELDS, analyzer, boosting);
        }
        private void LoadWordNet()
        {
            string directory = WORDNETDB_PATH;//Directory.GetCurrentDirectory();
            wordNet = new WordNetEngine();
            Console.WriteLine("Loading database...");
            wordNet.LoadFromDirectory(directory);
            Console.WriteLine("Load completed.");

            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.adj")), PartOfSpeech.Adjective);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.adv")), PartOfSpeech.Adverb);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.noun")), PartOfSpeech.Noun);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.verb")), PartOfSpeech.Verb);

            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.adj")), PartOfSpeech.Adjective);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.adv")), PartOfSpeech.Adverb);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.noun")), PartOfSpeech.Noun);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.verb")), PartOfSpeech.Verb);

            Console.WriteLine("Loading database...");
            wordNet.Load();
            Console.WriteLine("Load completed.");

        }
        protected string[] TokeniseString(string text)
        {
            char[] splitters = new char[] { ' ', '_', '/', '\t', '\'', '"', '-', '(', ')', ',', '’', '\n', ':', ';', '?', '.', '!' };
            return text.ToLower().Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        }
        private string SetWeightOriginalTrems(string [] orgs, List<string> tokens)
        {
            if (SettingsViewModel.Instance.QueryBoosting == false) return string.Join(", ", tokens);
            List<string> qlist = new List<string>();
            foreach (string token in tokens)
            {
                bool isFound = false;
                foreach(string org in orgs)
                {
                    if (org == token)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (isFound == true)
                    qlist.Add(token + "^2");
                else
                    qlist.Add(token);
            }
            return string.Join(", ", qlist);
        }
        protected string GetSynonyms(string query)
        {
            if (SettingsViewModel.Instance.PosLists.Length == 0) return query;

            List<string> synonymList = new List<string>();
            string[] tokens = TokeniseString(query.ToLower());
            
            foreach (var t in tokens)
            {
                var synSetList = wordNet.GetSynSets(t, SettingsViewModel.Instance.PosLists);

                if (synSetList.Count == 0)
                {
                    synonymList.Add(t);
                    Console.WriteLine($"No SynSet found for '{t}'");
                }

                foreach (var synSet in synSetList)
                {
                    if (synSet.PartOfSpeech == PartOfSpeech.None) continue;
                    
                    var words = string.Join(", ", synSet.Words);                    
                    synonymList.AddRange(synSet.Words);

                    //Console.WriteLine($"\nWords: {synSet.Words}");
                    //Console.WriteLine($"POS: {synSet.PartOfSpeech}");
                    //Console.WriteLine($"Gloss: {synSet.Gloss}");
                }
            }

            synonymList = synonymList.Distinct().ToList();

            //Console.WriteLine($"synonym List: {string.Join(", ", synonymList)}");
            return SetWeightOriginalTrems(tokens, synonymList);
        }
        private string[] StemTokens(string[] tokens)
        {
            int numTokens = tokens.Count();
            string[] stems = new string[numTokens];
            for (int i = 0; i < numTokens; i++)
            {
                stems[i] = porterStemmer.stemTerm(tokens[i]);
            }
            return stems;
        }
        private string Stemming(string query)
        {
            if (SettingsViewModel.Instance.Stemming == false) return query;

            string[] tokens = TokeniseString(query);
            //string[] tokensNoStop = StopWordFilter(tokens);
            string[] stems = StemTokens(tokens);
            Console.WriteLine($"==>Stemming : {string.Join(" ", stems)}");
            return string.Join(" ", stems);
        }
        protected override string PreProcessQuery(string query)
        {
            parser = CreateQueryParser();
            //return Stemming(GetSynonyms(query)) ;
            return GetSynonyms(query);
        }
        protected string UrlToString(string url)
        {
            string[] tokens = TokeniseString(url);
            return string.Join(" ", tokens);
        }
        override protected void IndexText(string isselected, string id, string url, string text)
        {
            Lucene.Net.Documents.Field field = new Field(TEXT_FN, text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS);
            Lucene.Net.Documents.Document doc = new Document();
            doc.Add(new Field(PID_FN, id, Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
            Lucene.Net.Documents.Field furl = new Field(URL_FN, UrlToString(url), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS);
            furl.Boost = 2f;
            doc.Add(furl);
            doc.Add(field);
            
            writer.AddDocument(doc);
        }
    protected override void CreateIndex(string indexPath)
        {
            luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
            writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
            writer.SetSimilarity(similarity);
        }                
    }
}
