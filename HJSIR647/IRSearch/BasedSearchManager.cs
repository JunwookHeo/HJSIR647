using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Search;

namespace HJSIR647.IRSearch
{
    class BasedSearchManager : SearchManager
    {
        public BasedSearchManager() : base()
        {
            InitLucene();
        }
        private void InitLucene()
        {
            analyzer = new Lucene.Net.Analysis.SimpleAnalyzer();
            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TEXT_FN, analyzer);

        }
        protected override string PreProcessQuery(string query)
        {
            return query;
        }
        override protected void IndexText(string isselected, string id, string url, string text)
        {
            Lucene.Net.Documents.Field field = new Field(TEXT_FN, url + " " + text, Field.Store.NO, Field.Index.ANALYZED_NO_NORMS, Field.TermVector.NO);
            Lucene.Net.Documents.Document doc = new Document();
            doc.Add(new Field(PID_FN, id, Field.Store.YES, Field.Index.NO, Field.TermVector.NO));
            doc.Add(field);

            writer.AddDocument(doc);
        }

        protected override void CreateIndex(string indexPath)
        {
            luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
            writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
        }

        override protected void CreateSearcher()
        {
            searcher = new IndexSearcher(luceneIndexDirectory);
        }
    }
}
