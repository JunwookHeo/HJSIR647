using HJSIR647.Collection;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class IrRelevantProvider : RelevantProvider, IDisposable
    {
        Lucene.Net.Store.Directory luceneIndexDirectory;
        Lucene.Net.Analysis.Analyzer analyzer;
        Lucene.Net.Index.IndexWriter writer;
        IndexSearcher searcher;
        QueryParser parser;

        const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
        const string TEXT_FN = "Text";
        const string QID_FN = "QID";
        const float threshHold = 1.0f;

        public IrRelevantProvider(CollectionProvider cp) : base(cp)
        {
            luceneIndexDirectory = null;
            writer = null;
            analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(VERSION);  //SimpleAnalyzer();
            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TEXT_FN, analyzer);

            CreateIndex();
            AddDocuments();
            CleanUpIndexer();
            CreateSearcher();
        }

        ~IrRelevantProvider()
        {
            CleanUpSearcher();
        }

        private void CreateIndex()
        {
            luceneIndexDirectory = new Lucene.Net.Store.RAMDirectory();
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
            writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
        }

        private void IndexText(long id, string text)
        {

            Lucene.Net.Documents.Document doc = new Document();
            doc.Add(new Field(QID_FN, id.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
            doc.Add(new Field(TEXT_FN, text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));

            writer.AddDocument(doc);
        }

        private void CleanUpIndexer()
        {
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Dispose();
        }

        private void CreateSearcher()
        {
            searcher = new IndexSearcher(luceneIndexDirectory);
        }
        private void AddDocuments()
        {
            foreach (CollectionQuery p in collectionProvider.Querys.Values)
            {
                IndexText(p.query_id, p.query);
            }
        }
        private void CleanUpSearcher()
        {
            searcher.Dispose();
        }

        //override public List<long> GetRelevantDocs(string querytext)
        override public Tuple<long, List<long>> GetRelevantDocs(string querytext)
        {
            long matched_query = 0;
            querytext = querytext.ToLower();
            Query query = parser.Parse(querytext);
            TopDocs results = searcher.Search(query, 10000);

            long rank = 0;
            float toprank = 0f;
            List<long> revevantList = new List<long>();

            foreach (ScoreDoc scoreDoc in results.ScoreDocs)
            {
                if (scoreDoc.Score < threshHold) break;

                Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);
                long id = Convert.ToInt64(doc.Get(QID_FN).ToString());
                string text = doc.Get(TEXT_FN).ToString();
                CollectionQuery cq = collectionProvider.Querys[id];
                Dictionary<long, CollectionPassage> cpd = collectionProvider.Passages;

                if (rank == 0)
                {
                    toprank = scoreDoc.Score;
                    matched_query = cq.query_id;
                }
                rank++;
                

                foreach (var pid in cq.passages_id)
                {
                    if (cpd[pid].is_selected == 1)
                    {
                        revevantList.Add(pid);
                    }
                }
                Console.WriteLine(rank.ToString() + ", " + text + "(" + scoreDoc.Score.ToString("0.000") + ":" +
                    (scoreDoc.Score / toprank).ToString("0.000") + ") ");
            }

            Console.WriteLine(string.Join(", ", revevantList));

            //return revevantList;
            return new Tuple<long, List<long>>(matched_query, revevantList);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                searcher.Dispose();
                writer.Dispose();                
                analyzer.Dispose();
                luceneIndexDirectory.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
