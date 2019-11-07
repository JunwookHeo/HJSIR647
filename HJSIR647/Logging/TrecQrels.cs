using HJSIR647.Collection;
using HJSIR647.Set;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Logging
{
    class TrecQrelsItem
    {
        long Qid;
        long Iter;
        long Docno;
        long Rel;

        public TrecQrelsItem(long qid, long iter, long docno, long rel)
        {
            Qid = qid;
            Iter = iter;
            Docno = docno;
            Rel = rel;
        }
        public string GetQrelsString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}", Qid.ToString("00000"), Iter.ToString("0"),Docno.ToString(), Rel.ToString("0"));
        }
    }
    class TrecQrels
    {
        static string PATH = @"../../out/trec_qrels.txt";
        public TrecQrels()
        {
            FileInfo f = new FileInfo(PATH);
            f.Directory.Create();
        }
        public void CreateQrels()
        { 
            List<JsonQuery> collections;
            string path = SettingsViewModel.Instance.CollectionPath;
            StreamReader file = null;

            try
            {
                file = File.OpenText(path);
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    file = null;
                    JArray array = (JArray)JToken.ReadFrom(reader);
                    collections = array.ToObject<List<JsonQuery>>();

                    using (StreamWriter sw = new StreamWriter(PATH))
                    {
                        foreach (var q in collections)
                        {
                            foreach (var p in q.passages)
                            {
                                if(p.is_selected == 1)
                                {
                                    TrecQrelsItem tqi = new TrecQrelsItem(q.query_id, 0, p.passage_ID, 1);
                                    sw.NewLine = "\n";
                                    sw.WriteLine(tqi.GetQrelsString());
                                }
                                
                            }

                        }
                    }
                        
                    Console.WriteLine("==> Completed saving Qrels");

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
