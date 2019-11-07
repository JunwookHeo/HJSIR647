using HJSIR647.Collection;
using HJSIR647.Logging;
using HJSIR647.Performance;
using HJSIR647.Set;
using HJSIR647.StatusBar;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IFormatter = Lucene.Net.Search.Highlight.IFormatter;

namespace HJSIR647.IRSearch
{
    abstract class SearchManager
    {
        protected Lucene.Net.Store.Directory luceneIndexDirectory;
        protected Lucene.Net.Analysis.Analyzer analyzer;
        protected Lucene.Net.Index.IndexWriter writer;
        protected IndexSearcher searcher;
        protected QueryParser parser;
        SettingsViewModel settingsVM;
        RelevantProvider relevantProvider;
        TrecLogger trecLogger;

        protected const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
        protected static string TEXT_FN = SettingsViewModel.TEXT;
        protected static string PID_FN = "PID";
        protected const int MAX_QUERY = 50;//10000;

        CollectionProvider collectionProvider;

        public SearchManager()
        {
            settingsVM = SettingsViewModel.Instance; ;
            luceneIndexDirectory = null;
                        
            collectionProvider = new CollectionProvider(settingsVM.CollectionPath);
            relevantProvider = new SimpleRelevantProvider(collectionProvider);

            trecLogger = new TrecLogger();            
        }
        
        ~SearchManager()
        {            
            CleanUpSearcher();
            Console.WriteLine("Finalized SearchManager");
        }

        public void UpdateCollectionProvider(object sender = null)
        {
            StatusBarViewModel.Instance.ProcStatus = "Loading Indexing File .....";
            collectionProvider = new CollectionProvider(settingsVM.CollectionPath, sender);
            StatusBarViewModel.Instance.ProcStatus = "Finish Loading Indexing File";

            relevantProvider = new SimpleRelevantProvider(collectionProvider);
        }
        /// <summary>
        /// Creates the index at a given path
        /// </summary>
        /// <param name="indexPath">The pathname to create the index</param>
        abstract protected void CreateIndex(string indexPath);

        /// <summary>
        /// Indexes a given string into the index
        /// </summary>
        /// <param name="text">The text to index</param>
        abstract protected void IndexText(string isselected, string id, string url, string text);


        /// <summary>
        /// Flushes the buffer and closes the index
        /// </summary>
        private void CleanUpIndexer()
        {
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Dispose();
        }


        /// <summary>
        /// Creates the searcher object
        /// </summary>
        abstract protected void CreateSearcher();

        /// <summary>
        /// Searches the index for the querytext and displays a ranked list of results to the screen
        /// </summary>
        /// <param name="querytext">The text to search the index</param>
        private string SearchAndDisplayResults(string querytext, long qid, List<long> relevantList)
        {

            System.Console.WriteLine("Searching for " + querytext);
            querytext = querytext.ToLower();
            Query query = parser.Parse(querytext);

            System.Console.WriteLine($"Searching for { query.ToString()}");

            TopDocs results = searcher.Search(query, MAX_QUERY);

            // create highlighter - using strong tag to highlight in this case (change as needed)
            //IFormatter formatter = new SimpleHTMLFormatter("<strong>", "</strong>");
            IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;background-color:yellow;\">", "</span>");

            // excerpt set to 200 characters in length
            var fragmenter = new SimpleFragmenter(3000);
            var scorer = new QueryScorer(query);
            var highlighter = new Highlighter(formatter, scorer) { TextFragmenter = fragmenter };
            
            long rank = 0;
            float topscore = 0f;
            long foundrelevants = 0;
            List<TrecItem> logItems = new List<TrecItem>();
            SearchedListViewModel.DeleteAll();
            foreach (ScoreDoc scoreDoc in results.ScoreDocs)
            {
                if (rank == 0) topscore = scoreDoc.Score;
                rank++;
                Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);
                long id = Convert.ToInt64(doc.Get(PID_FN).ToString());
                CollectionPassage ps = collectionProvider.Passages[id];

                // Logging Trec
                logItems.Add(new TrecItem(0, id, rank, scoreDoc.Score) );

                // get highlighted fragment
                TokenStream stream = analyzer.TokenStream("", new StringReader(ps.passage_text));
                string highlighted = highlighter.GetBestFragment(stream, ps.passage_text);

                //string url2 = doc.Get(TEXT_FN).ToString();
                //Console.WriteLine("Rank " + rank + " text " + myFieldValue);
                if(highlighted == null)
                {
                    highlighted = ps.passage_text;
                }
                if (relevantList.Contains(id)) foundrelevants++;
                SearchedListViewModel.Add(scoreDoc.Score/topscore, id, ps.GetTitle(), ps.url, highlighted, relevantList.Contains(id));
                               
                //Console.WriteLine("==>" + highlighted);
            }

            StatusBarViewModel.Instance.NumRelevants = "Num Relevants : " + foundrelevants.ToString() + "/" + relevantList.Count.ToString();
            StatusBarViewModel.Instance.NumSearch = "Num Searched :" + results.ScoreDocs.Length.ToString();
            // Logging Trec
            trecLogger.Logging(qid, logItems);

            //Console.WriteLine(string.Join(",", relevantList));            
            return query.ToString();
        }
        
        /// <summary>
        /// Closes the index after searching
        /// </summary>
        private void CleanUpSearcher()
        {
            try
            {
                searcher.Dispose();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        protected abstract string PreProcessQuery(string query);

        private void AddDocuments(object sender=null)
        {
            int i = 0;
            int total = collectionProvider.Passages.Values.Count;
            
            foreach (CollectionPassage p in collectionProvider.Passages.Values)
            {                
                IndexText(p.is_selected.ToString(), p.passage_ID.ToString(), p.url, p.passage_text);
                if(sender != null && i++%100 == 0)
                {
                    (sender as BackgroundWorker).ReportProgress(100*i/ total);                    
                }                
            }
        }

        internal void IndexProcRun(object sender=null)
        {
            // Create Index
            Console.WriteLine("Creating Idex");
            CreateIndex(settingsVM.IndexPath);
            // Add Documents
            Console.WriteLine("Adding Documents");
            AddDocuments(sender);
            // CleanUp Indexer
            Console.WriteLine("Cleaning up Indexer");
            CleanUpIndexer();
            // Searching Code
            Console.WriteLine("Creating Searcher");
            CreateSearcher();
        }

        public void IndexProc(object sender=null)
        {
            StatusBarViewModel.Instance.ProcStatus = "Start Indexing...";
            DateTime start = System.DateTime.Now;

            IndexProcRun(sender);

            DateTime end = System.DateTime.Now;
            PerformanceViewModel pvm = PerformanceViewModel.Instance;
            double interval = (end - start).TotalMilliseconds;
            pvm.AddIndexTime(interval, settingsVM.AdvancedSearch);

            StatusBarViewModel.Instance.ProcStatus = string.Format("Finish Indexing : {0}ms", interval.ToString("0.000"));
        }

        internal void SearchProcRun(string input, long qid, List<long> relevantList)
        {
            if (settingsVM.NoPreProcessing == true)
            {
                input = "\"" + input + "\"";
            }
            else
            {                
                if (searcher != null)
                {
                    input = PreProcessQuery(input);
                }
                
            }

            string query = SearchAndDisplayResults(input, qid, relevantList);
            SearchProcViewModel.Instance.ProcessedQuery = query;            
        }

        public void SearchProc(string input)
        {
            Tuple < long, List<long>> relevantList = relevantProvider.GetRelevantDocs(input);
            StatusBarViewModel.Instance.ProcStatus = "Start Searching...";
            DateTime start = System.DateTime.Now;

            SearchProcRun(input, relevantList.Item1, relevantList.Item2);

            DateTime end = System.DateTime.Now;
            double interval = (end - start).TotalMilliseconds;
            PerformanceViewModel pvm = PerformanceViewModel.Instance;
            pvm.AddSearchTime(interval, settingsVM.AdvancedSearch);

            StatusBarViewModel.Instance.ProcStatus = string.Format("Finish Searching : {0}ms", interval.ToString("0.000"));
        }

        public string GetPassageDetails(long pid, string passage)
        {
            string details = "<html>";
            CollectionPassage cp = collectionProvider.GetPassageDetails(pid);
                        
            details += "<body";
            details += "<title>" + cp.GetTitle() + "</br></title>";
            details += "Passage ID : " + cp.passage_ID.ToString() + "</br>";
            details += "is_selected : " + cp.is_selected.ToString() + "</br>";
            details += "URL : " + cp.url + "</br>";
            details += "Query ID : " + cp.query_ID.ToString() + "</br>";
            details += passage + "</br>";
            details += "</body";
            details += "</html>";
            return details;
        }

    }
}
