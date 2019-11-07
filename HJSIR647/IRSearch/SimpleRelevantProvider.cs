using HJSIR647.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class SimpleRelevantProvider : RelevantProvider
    {
        public SimpleRelevantProvider(CollectionProvider cp) : base(cp)
        {
        }

        override public Tuple<long, List<long> > GetRelevantDocs(string querytext)
        {
            long matched_query = 0;
            List<long> revevantList = new List<long>();

            foreach (KeyValuePair<long, CollectionQuery> cq in collectionProvider.Querys)
            {
                if (querytext == cq.Value.query)
                {
                    Dictionary<long, CollectionPassage> cpd = collectionProvider.Passages;

                    matched_query = cq.Value.query_id;
                    foreach (var pid in cq.Value.passages_id)
                    {
                        if (cpd[pid].is_selected == 1)
                        {
                            revevantList.Add(pid);
                        }
                    }
                }                
            }

            Console.WriteLine(string.Join(", ", revevantList));

            //return revevantList;
            return new Tuple<long, List<long> >(matched_query, revevantList);
        }
    }
}
