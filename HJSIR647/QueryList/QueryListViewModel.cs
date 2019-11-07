using HJSIR647.Collection;
using HJSIR647.Set;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.QueryList
{
    class QueryListView
    {
        public long QueryID { get; set; }
        public string QueryType { get; set; }
        public string Query { get; set; }

        public QueryListView(long queryId, string queryType, string query)
        {
            QueryID = queryId;
            QueryType = queryType;
            Query = query;
        }
    }

    class QueryListViewModel : ObservableCollection<QueryListView>
    {
        public QueryListViewModel()
        {
            List<JsonQuery> collections;
            string path = SettingsViewModel.Instance.CollectionPath;
            StreamReader file = null;
            try
            {
                // read JSON directly from a file
                file = File.OpenText(path);
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    file = null;
                    JArray array = (JArray)JToken.ReadFrom(reader);
                    collections = array.ToObject<List<JsonQuery>>();

                    foreach(var q in collections)
                    {
                        this.Add(new QueryListView(q.query_id, q.query_type, q.query));
                    }
                    Console.WriteLine(collections.Count.ToString());
                    
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                file?.Dispose();
            }
        }
    }
}
