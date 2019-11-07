using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.StatusBar
{
    class StatusBarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string procStatus;
        private string numRelevants;
        private string numSearch;

        private static StatusBarViewModel instance = null;

        private StatusBarViewModel()
        {
        }

        public static StatusBarViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StatusBarViewModel();
                }
                return instance;
            }
        }
        public string ProcStatus
        {
            get { return procStatus; }
            set
            {
                procStatus = value;
                OnPropertyChanged("ProcStatus");
            }
        }
        public string NumRelevants
        {
            get { return numRelevants; }
            set
            {
                numRelevants = value;
                OnPropertyChanged("NumRelevants");
            }
        }
        public string NumSearch
        {
            get { return numSearch; }
            set
            {
                numSearch = value;
                OnPropertyChanged("NumSearch");
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
