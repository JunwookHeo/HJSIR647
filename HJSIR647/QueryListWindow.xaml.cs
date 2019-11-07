using HJSIR647.Collection;
using HJSIR647.QueryList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HJSIR647
{
    /// <summary>
    /// Interaction logic for QueryListWindow.xaml
    /// </summary>
    public partial class QueryListWindow : Window
    {
        QueryListViewModel queryListViewModel;

        public delegate void QueryListEventHanler(object sender, EventArgs e);
        public event QueryListEventHanler SearchQueryEventHandler;

        public QueryListWindow()
        {
            InitializeComponent();
            // Add to close this window when the mainwindow closes
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Closed += (s, e) => Close();
            }
                

            LoadQueryList();
        }

        private void LoadQueryList()
        {
            queryListViewModel = new QueryListViewModel();
            queryListView.ItemsSource = queryListViewModel;
        }

        private void QueryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                QueryListView item = (QueryListView)queryListView.SelectedItem;
                Console.WriteLine(item.Query);
                this.SearchQueryEventHandler(this, new QueryListEventArgs(item.Query));
            }
        }
    }

    public class QueryListEventArgs : EventArgs
    {
        public string QueryListEventString { get; set; }

        public QueryListEventArgs(string query)
        {
            this.QueryListEventString = query;
        }
    }
}
