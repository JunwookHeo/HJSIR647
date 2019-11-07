using System;

namespace HJSIR647.Collection
{
    class CollectionPassage
    {
        public int is_selected { get; private set; }
        public string url { get; private set; }
        public string passage_text { get; private set; }
        public long passage_ID { get; private set; }
        public long query_ID { get; private set; }

        public CollectionPassage(JsonPassage jp, long queryid)
        {
            is_selected = jp.is_selected;
            url = jp.url;
            passage_text = jp.passage_text;
            passage_ID = jp.passage_ID;
            query_ID = queryid;
        }

        public string GetTitle()
        {
            Uri uri = new Uri(url);
            return uri.Host;
        }
    }
}
