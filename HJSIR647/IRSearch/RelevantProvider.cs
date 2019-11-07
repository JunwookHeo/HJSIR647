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
    abstract class RelevantProvider
    {
        protected CollectionProvider collectionProvider;
        public RelevantProvider(CollectionProvider cp)
        {
            collectionProvider = cp;
        }
        //abstract public List<long> GetRelevantDocs(string querytext);
        abstract public Tuple<long, List<long>> GetRelevantDocs(string querytext);
    }
}
