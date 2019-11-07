using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace HJSIR647.Collection
{
    class CollectionProvider
    {
        public Dictionary<long, CollectionPassage> Passages { get; private set; }
        public Dictionary<long, CollectionQuery> Querys { get; private set; }

        public CollectionProvider(string path, object sender = null)
        {
            Passages = new Dictionary<long, CollectionPassage>();
            Querys = new Dictionary<long, CollectionQuery>();

            if (sender != null)
            {
                (sender as BackgroundWorker).ReportProgress(50);
            }

            //path = @"../../Dataset/collection.json";
            List<JsonQuery> collections = LoadCollectionJson(path);
            //SaveSamplesJson(collections);

            if(collections != null)
                CreateDB(collections, sender);
            else
                System.Windows.Forms.MessageBox.Show($"Cannot load {path}", "Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private List<JsonQuery> LoadCollectionJson(string path)
        {
            List<JsonQuery> collections;
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

                    Console.WriteLine(collections.Count.ToString());
                    return collections;
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                file?.Dispose();
            }
            return null;
        }
        private void SaveSamplesJson(List<JsonQuery> collections)
        {
            using (StreamWriter file = File.CreateText(@"../../samples.json"))
            {
                List<JsonQuery> passage = new List<JsonQuery>();
                int i = 0;
                foreach (JsonQuery c in collections)
                {
                    passage.Add(c);
                    if (i++ > 1000) break;
                }

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, passage);
            }
        }
        private void CreateDB(List<JsonQuery> collections, object sender = null)
        {
            int i = 0;
            int total = collections.Count;
            
            foreach (JsonQuery jq in collections)
            {
                CollectionQuery cq = new CollectionQuery(jq);
                
                foreach (var jp in jq.passages)
                {
                    CollectionPassage cp = new CollectionPassage(jp, jq.query_id);
                    Passages.Add(cp.passage_ID, cp);

                    cq.AddPassagesID(jp.passage_ID);
                }

                Querys.Add(cq.query_id, cq);
                if (sender != null && i++ % 100 == 0)
                {
                    (sender as BackgroundWorker).ReportProgress(100 * i / total);
                }
            }
        }

        public CollectionPassage GetPassageDetails(long pid)
        {
            return Passages[pid];
        }
    }
}
