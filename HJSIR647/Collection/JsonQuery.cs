using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Collection
{
    
    class JsonQuery
    {
        public List<JsonPassage> passages { get; set; }
        public long query_id { get; set; }
        public List<string> answers { get; set; }
        public string query_type { get; set; }
        public string query { get; set; }

    }
}
