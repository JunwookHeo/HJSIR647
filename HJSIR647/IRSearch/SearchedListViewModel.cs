using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class SearchedListViewModel
    {
        public string Rank { get; set; }
        public long PassageID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Passage { get; set; }
        public bool IsRelevantDoc { get; set; }

        private static ObservableCollection<SearchedListViewModel> instance;

        public static ObservableCollection<SearchedListViewModel> GetInstance()
        {
            if (instance == null)
                instance = new ObservableCollection<SearchedListViewModel>();
            return instance;
        }

        public static void Add(float rank, long passageId, string atitle, string aurl, string apassage, bool isRelevantDoc)
        {
            try {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    GetInstance().Add(new SearchedListViewModel() { Rank = (rank * 100).ToString("0.00"), PassageID = passageId,
                        Title = atitle, Url = aurl, Passage = apassage, IsRelevantDoc = isRelevantDoc });
                });
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public static void DeleteAll()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                GetInstance().Clear();
            });            
        }                
    }
}
