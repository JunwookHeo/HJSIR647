using HJSIR647.Set;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Logging
{
    class TrecLogger
    {
        //const string PATHLOG = @"../../out/trec_log.txt";
        private long TopicID = 90000;

        public void Logging(long topicId, List<TrecItem> lists)
        {
            if (SettingsViewModel.Instance.LoggingTrec == false) return;

            FileInfo file = new FileInfo(SettingsViewModel.Instance.LoggingPath);
            file.Directory.Create();
            if(topicId == 0)
                topicId = TopicID++;

            using (StreamWriter sw = new StreamWriter(SettingsViewModel.Instance.LoggingPath, append:true))
            {
                foreach(var l in lists)
                {
                    sw.NewLine = "\n";
                    sw.WriteLine (l.GetLogString(topicId));
                }
                
            }
        }
    }
}
