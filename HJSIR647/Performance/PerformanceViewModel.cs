using HJSIR647.Set;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJSIR647.Performance
{
    class PerformanceViewModel
    {
        public SeriesCollection IndexSeriesData { get; set; }
        public SeriesCollection SearchSeriesData { get; set; }

        private LineSeries blSearchPerformance;
        private LineSeries blIndexPerformance;
        private LineSeries adSearchPerformance;
        private LineSeries adIndexPerformance;

        private static PerformanceViewModel instance = null;

        private const string BLS_TXT = "Base Line Searching";
        private const string BLI_TXT = "Base Line Indexing";
        private const string ADS_TXT = "Advanced Searching";
        private const string ADI_TXT = "Advanced Indexing";

        private PerformanceViewModel()
        {
            IndexSeriesData = new SeriesCollection();
            SearchSeriesData = new SeriesCollection();

            blSearchPerformance = new LineSeries();
            blSearchPerformance.Values = new ChartValues<ObservablePoint>();
            blSearchPerformance.Title = BLS_TXT; // "Base Line Searching";
            blSearchPerformance.Fill = System.Windows.Media.Brushes.Transparent;
            SearchSeriesData.Add(blSearchPerformance);

            blIndexPerformance = new LineSeries();
            blIndexPerformance.Values = new ChartValues<ObservablePoint>();
            blIndexPerformance.Title = BLI_TXT; // "Base Line Indexing";
            blIndexPerformance.Fill = System.Windows.Media.Brushes.Transparent;
            IndexSeriesData.Add(blIndexPerformance);

            adSearchPerformance = new LineSeries();            
            adSearchPerformance.Values = new ChartValues<ObservablePoint>();
            adSearchPerformance.Title = ADS_TXT;// "Advanced Searching";
            adSearchPerformance.Fill = System.Windows.Media.Brushes.Transparent;
            SearchSeriesData.Add(adSearchPerformance);

            adIndexPerformance = new LineSeries();
            adIndexPerformance.Values = new ChartValues<ObservablePoint>();
            adIndexPerformance.Title = ADI_TXT; // "Advanced Indexing";
            adIndexPerformance.Fill = System.Windows.Media.Brushes.Transparent;
            IndexSeriesData.Add(adIndexPerformance);
        }
        public static PerformanceViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PerformanceViewModel();
                }
                return instance;
            }
        }
        private double GetAverage(IChartValues values)
        {
            double avg = 0f;
            double sum = 0f;
            foreach(ObservablePoint v in values)
            {
                sum += v.Y;
            }
            avg = sum / values.Count;

            return avg;
        }

        private void UpdateAverage(string title, LineSeries series)
        {
            App.Current.Dispatcher.Invoke(() => {
                series.Title = $"{title} ({GetAverage(series.Values).ToString("0.000")})";
            });
        }

        public void AddSearchTime(double time, bool advanced)
        {
            if(SettingsViewModel.Instance.LoggingGraph == true)
            {
                if (advanced == true)
                {
                    adSearchPerformance.Values.Add(new ObservablePoint(adSearchPerformance.Values.Count + 1, time));
                    UpdateAverage(ADS_TXT, adSearchPerformance);
                }                    
                else
                {
                    blSearchPerformance.Values.Add(new ObservablePoint(blSearchPerformance.Values.Count + 1, time));
                    UpdateAverage(BLS_TXT, blSearchPerformance);
                }
                    
               
            }            
        }
        public void AddIndexTime(double time, bool advanced)
        {
            if (SettingsViewModel.Instance.LoggingGraph == true)
            {
                if (advanced == true)
                {
                    adIndexPerformance.Values.Add(new ObservablePoint(adIndexPerformance.Values.Count + 1, time));
                    UpdateAverage(ADI_TXT, adIndexPerformance);
                }                    
                else
                {
                    blIndexPerformance.Values.Add(new ObservablePoint(blIndexPerformance.Values.Count + 1, time));
                    UpdateAverage(BLI_TXT, blIndexPerformance);
                }                    
            }
        }

        public void ClearIndexChart()
        {
            blIndexPerformance.Values.Clear();
            blIndexPerformance.Title = "Base Line Indexing";
            adIndexPerformance.Values.Clear();
            adIndexPerformance.Title = "Advanced Indexing";
        }

        public void ClearSearchChart()
        {
            blSearchPerformance.Values.Clear();
            blSearchPerformance.Title = "Base Line Searching";
            adSearchPerformance.Values.Clear();
            adSearchPerformance.Title = "Advanced Searching";
        }
    }
}
