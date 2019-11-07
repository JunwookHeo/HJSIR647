using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.IRSearch
{
    class SearchProcViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string processedQuery;

        private static SearchProcViewModel instance = null;

        private SearchProcViewModel()
        {
        }

        public static SearchProcViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SearchProcViewModel();
                }
                return instance;
            }
        }

        public string ProcessedQuery
        {
            get { return processedQuery; }
            set
            {
                processedQuery = value;
                OnPropertyChanged("ProcessedQuery");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
