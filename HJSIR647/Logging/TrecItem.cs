using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Logging
{
    class TrecItem
    {
        private long TopicID;
        private string Q0;
        private long DocID;
        private long Rank;
        private float Score;
        private string Signature;

        const string Q0_HJS = "Q0";
        const string SIG_HJS = "10150765_10298851_10105981_HJSIR647";
        public TrecItem(long topicId, long docId, long rank, float score)
        {
            TopicID = topicId;
            Q0 = Q0_HJS;
            DocID = docId;
            Rank = rank;
            Score = score;
            Signature = SIG_HJS;
        }
        public string GetLogString(long topicId)
        {
            TopicID = topicId;
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", TopicID.ToString("00000"), Q0, DocID.ToString(), 
                        Rank.ToString(), Score.ToString(".000000"), Signature);
        }
    }
}
