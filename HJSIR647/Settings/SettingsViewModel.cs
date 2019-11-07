using Syn.WordNet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HJSIR647.Set
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string collectionPath;
        private string indexPath;
        private bool noPreProcessing;
        private bool advancedSearch;
        private bool loggingTrec;
        private bool loggingGraph;
        private string loggingPath;

        private bool synAdjective;
        private bool synAdverb;
        private bool synNoun;
        private bool synVerb;

        private bool stopWords;
        private bool stemming;
        private bool queryBoosting;

        public static string NONE = "None";
        public static string URL = "URL";
        public static string TEXT = "Text";
        private string selectedFieldBoosting;

        private static SettingsViewModel instance = null;

        public ObservableCollection<string> fieldBoostingList;

        private SettingsViewModel()
        {
            // Download collection.json and copy it to Dataset
            // @"https://www.dropbox.com/s/h44lap020wcb0hh/collection.json?dl=0"

            //CollectionPath = @"../../Dataset/Collection.json";
            // Load Samples.json
            CollectionPath = @"../../Dataset/samples.json";
            IndexPath = @"../../Index";
            LoggingPath = @"../../out/trec_log.txt";
            SynAdjective = false;
            SynAdverb = false;
            SynNoun = true;
            SynVerb = false;

            StopWords = true;
            Stemming = true;

            SelectedFieldBoosting = NONE;
            FieldBoostingList = new ObservableCollection<string>()
            {
                {NONE},
                {URL},
                {TEXT }
            };
        }
        public static SettingsViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsViewModel();
                }
                return instance;
            }
        }
        public string CollectionPath
        {
            get
            {
                return collectionPath;
            }
            set
            {
                collectionPath = value;
                OnPropertyChanged("CollectionPath");
            }
        }
        public string IndexPath
        {
            get
            { return indexPath; }
            set
            {
                indexPath = value;
                OnPropertyChanged("IndexPath");
            }
        }
        public bool NoPreProcessing
        {
            get
            { return noPreProcessing; }
            set
            {
                noPreProcessing = value;
                OnPropertyChanged("NoPreProcessing");
            }
        }
        public bool AdvancedSearch
        {
            get
            { return advancedSearch; }
            set
            {
                advancedSearch = value;
                OnPropertyChanged("NoPreProcessing");
            }
        }
        public bool LoggingTrec
        {
            get
            { return loggingTrec; }
            set
            {
                loggingTrec = value;
                OnPropertyChanged("LoggingTrec");
            }
        }

        public bool LoggingGraph
        {
            get
            { return loggingGraph; }
            set
            {
                loggingGraph = value;
                OnPropertyChanged("LoggingGraph");
            }
        }
        public string LoggingPath
        {
            get
            { return loggingPath; }
            set
            {
                loggingPath = value;
                OnPropertyChanged("LoggingPath");
            }
        }

        public bool SynAdjective
        {
            get
            { return synAdjective; }
            set
            {
                synAdjective = value;                
                OnPropertyChanged("SynAdjective");
            }
        }

        public bool SynAdverb
        {
            get
            { return synAdverb; }
            set
            {
                synAdverb = value;                
                OnPropertyChanged("SynAdverb");
            }
        }
        public bool SynNoun
        {
            get
            { return synNoun; }
            set
            {
                synNoun = value;
                OnPropertyChanged("SynNoun");
            }
        }
        public bool SynVerb
        {
            get
            { return synVerb; }
            set
            {
                synVerb = value;
                OnPropertyChanged("SynVerb");
            }
        }
        public PartOfSpeech[] PosLists
        {
            get
            {
                List<PartOfSpeech> poslist = new List<PartOfSpeech>();
                if (synAdjective == true) poslist.Add(PartOfSpeech.Adjective);
                if (synAdverb == true) poslist.Add(PartOfSpeech.Adverb);
                if (synNoun == true) poslist.Add(PartOfSpeech.Noun);
                if (synVerb == true) poslist.Add(PartOfSpeech.Verb);
                return poslist.ToArray();
            }
        }

        public bool StopWords
        {
            get
            { return stopWords; }
            set
            {
                stopWords = value;
                OnPropertyChanged("StopWords");
            }
        }
        public bool Stemming
        {
            get
            { return stemming; }
            set
            {
                stemming = value;
                OnPropertyChanged("Stemming");
            }
        }

        public bool QueryBoosting
        {
            get
            { return queryBoosting; }
            set
            {
                queryBoosting = value;
                OnPropertyChanged("QueryBoosting");
            }
        }
        
        public string SelectedFieldBoosting
        {
            get
            { return selectedFieldBoosting; }
            set
            {
                selectedFieldBoosting = value;
                OnPropertyChanged("SelectedFieldBoosting");
            }
        }
        public ObservableCollection<string> FieldBoostingList
        {
            get { return fieldBoostingList; }
            set { fieldBoostingList = value; }
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
