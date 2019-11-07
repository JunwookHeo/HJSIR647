using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Collection
{
    class CollectionQuery
    {
        public List<long> passages_id { get; private set; }
        public long query_id { get; private set; }
        public List<string> answers { get; private set; }
        public string query_type { get; private set; }
        public string query { get; private set; }

        public CollectionQuery(JsonQuery jp)
        {
            query_id = jp.query_id;
            answers = jp.answers;
            query_type = jp.query_type;
            query = jp.query;

            passages_id = new List<long>();
        }
        public void AddPassagesID(long pid)
        {
            passages_id.Add(pid);
        }
    }
}
