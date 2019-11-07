using HJSIR647.Performance;
using HJSIR647.IRSearch;
using HJSIR647.Set;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using HJSIR647.StatusBar;
using Microsoft.Win32;
using System.Threading;
using System.ComponentModel;
using System.Windows.Input;
using HJSIR647.Collection;
using static HJSIR647.QueryListWindow;
using HJSIR647.Logging;

namespace HJSIR647
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>    
    public partial class MainWindow : Window, IDisposable
    {
        SettingsViewModel settingViewModel;
        PerformanceViewModel performanceViewModel;
        SearchManager searchManager;
        StatusBarViewModel statusBarViewModel;
        SearchProcViewModel searchProcViewModel;

        BackgroundWorker bworker;
        private static int usingResource = 0;
        public MainWindow()
        {
            InitializeComponent();

            // Binding Settings and SettingsViewModel
            settingViewModel = SettingsViewModel.Instance;
            SettingPanel.DataContext = settingViewModel;

            // Binding SearchedList and SearchListViewModel
            searchedListView.ItemsSource = SearchedListViewModel.GetInstance();

            // Binding performance and PerformanceViewModel
            performanceViewModel = PerformanceViewModel.Instance;
            gridPerformance.DataContext = performanceViewModel;

            // Binding sbStatusBar and StatusBarViewModel
            statusBarViewModel = StatusBarViewModel.Instance;
            sbStatusBar.DataContext = statusBarViewModel;

            // Binding sbStatusBar and SearchProcViewModel
            searchProcViewModel = SearchProcViewModel.Instance;
            gdSearchProc.DataContext = searchProcViewModel;

            // Create searchManager
            if(settingViewModel.AdvancedSearch == true)
                searchManager = new AdvancedSearchManager();
            else
                searchManager = new BasedSearchManager();
            searchManager.IndexProc();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            if (File.Exists(settingViewModel.CollectionPath) == false)
            {
                Load_Json_Dumy();
            }
        }

        private void Load_Json_Dumy()
        {
            SearchedListViewModel.Add(1, 2, "Select Collection.json", "Dataset/Collection.json", "copy to Dataset foler", false);
            //SearchedListViewModel.Add(2, 3, "Title 1", "http://title1.com", "this is title 1 passage.", true);
            //SearchedListViewModel.Add(3, 4, "Title 2", "http://title2.com", "this is title 2 passage.", false);           
        }

        private void SearchedListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SearchedListViewModel item = (SearchedListViewModel)searchedListView.SelectedItem;
                string details = searchManager.GetPassageDetails(item.PassageID, item.Passage);
                selectedItem.NavigateToString( details );                
            }                
        }

        private void btCollection_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (1 == Interlocked.Exchange(ref usingResource, 1)) return;
                Bw_Create_Worker();
                bworker.DoWork += Bw_btCollection_Click;
                bworker.RunWorkerAsync(dlg.FileName);
            }
            
        }

        private void btIndex_Click(object sender, RoutedEventArgs e)
        {            
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    settingViewModel.IndexPath = dlg.SelectedPath;
                }
            }

        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            if (tbInputQuery.Text != "")
            {
                if (1 == Interlocked.Exchange(ref usingResource, 1)) return;
                Bw_Create_Worker();
                bworker.DoWork += Bw_btSearch_Click;
                bworker.RunWorkerAsync(tbInputQuery.Text);
            }            
        }

        private void btIndexing_Click(object sender, RoutedEventArgs e)
        {
            if (1 == Interlocked.Exchange(ref usingResource, 1)) return;
            Bw_Create_Worker();
            bworker.DoWork += Bw_btIndexing_Click;
            bworker.RunWorkerAsync(usingResource);
        }

        private void cbAdvancedSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (1 == Interlocked.Exchange(ref usingResource, 1)) return;
            Bw_Create_Worker();
            bworker.DoWork += Bw_cbAdvancedSearch_Unchecked;
            bworker.RunWorkerAsync(usingResource);
        }

        private void cbAdvancedSearch_Checked(object sender, RoutedEventArgs e)
        {
            if (1 == Interlocked.Exchange(ref usingResource, 1)) return;
            Bw_Create_Worker();
            bworker.DoWork += Bw_cbAdvancedSearch_Checked;
            bworker.RunWorkerAsync(usingResource);
        }
        
        private void Bw_btCollection_Click(object sender, DoWorkEventArgs e)
        {
            string path = e.Argument as string;
            if (path == null) return;
            settingViewModel.CollectionPath = path;
            searchManager.UpdateCollectionProvider(sender);
        }
        private void Bw_btSearch_Click(object sender, DoWorkEventArgs e)
        {
            string query = e.Argument as string;
            if (query == null) return;
            searchManager.SearchProc(query);
        }
        
        private void Bw_btIndexing_Click(object sender, DoWorkEventArgs e)
        {
            searchManager.IndexProc(sender);            
        }
        private void Bw_cbAdvancedSearch_Unchecked(object sender, DoWorkEventArgs e)
        {
            // Create Advanced searchManager
            searchManager = new BasedSearchManager();
            searchManager.IndexProc(sender);
        }
        
        private void Bw_cbAdvancedSearch_Checked(object sender, DoWorkEventArgs e)
        {
            // Create Advanced searchManager
            searchManager = new AdvancedSearchManager();
            searchManager.IndexProc(sender);
        }

        //Taking your results
        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Interlocked.Exchange(ref usingResource, 0);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
            ProgressBarMain.Value = 0;
        }
        private void Bw_Create_Worker()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            bworker = new BackgroundWorker();
            bworker.WorkerReportsProgress = true;
            bworker.ProgressChanged += Bw_ProgressChanged;
            bworker.RunWorkerCompleted += Bw_RunWorkerCompleted;

            ProgressBarMain.Value = 0;
        }
        void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            ProgressBarMain.Value = e.ProgressPercentage;
         }
        private void MiQueryOpen_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            QueryListWindow queryListWin = new QueryListWindow();
            queryListWin.SearchQueryEventHandler += new QueryListEventHanler(btSearch_From_QueryListWindow);
            queryListWin.Show();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        private void btSearch_From_QueryListWindow(object sender, EventArgs e)
        {
            string query = ((QueryListEventArgs)e).QueryListEventString;
            if (query.ToString() != "")
            {             
                if (1 == Interlocked.Exchange(ref usingResource, 1)) return;

                tbInputQuery.Text = query;
                Bw_Create_Worker();
                bworker.DoWork += Bw_btSearch_Click;
                bworker.RunWorkerAsync(tbInputQuery.Text);
            }
        }

        private void MiClearIndexChart_Click(object sender, RoutedEventArgs e)
        {
            performanceViewModel.ClearIndexChart();
        }

        private void MiClearSearchChart_Click(object sender, RoutedEventArgs e)
        {
            performanceViewModel.ClearSearchChart();
        }

        private void btLogging_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == true)
            {
                settingViewModel.LoggingPath = dlg.FileName;
            }
        }

        private void MiCreateQrels_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });

            TrecQrels tq = new TrecQrels();
            tq.CreateQrels();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                bworker.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
