using AlinaLib.Domain.UseCase.DirectoryWatcher;
using System.Collections.ObjectModel;
using System.Windows;

namespace AlinaWpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object _locker = new object();

        private DirectoryWatcher _dirWatcher;

        public ObservableCollection<string> Folders { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> CsvFiles { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> XmlFiles { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> PairFiles { get; set; } = new ObservableCollection<string>();



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Folders.Add(AppDomain.CurrentDomain.BaseDirectory.ToString());
            Folders.Add(AppDomain.CurrentDomain.BaseDirectory.ToString());
            CsvLV.ItemsSource = CsvFiles;
            XmlLV.ItemsSource = XmlFiles;
            PairLV.ItemsSource = PairFiles;
            UpdateWatcher(Folders[0]);
        }

        private void ButtonIn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = Folders[0]
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (Folders[0] != dialog.SelectedPath)
            {
                ClearLists();
                Folders[0] = dialog.SelectedPath;
                UpdateWatcher(Folders[0]);
            }
        }

        private void UpdateUI()
        {
            if (_dirWatcher.GetCsvFilePathsWoPair().Count > 0)
            {
                InvokeAddOrChangeRange(CsvFiles, _dirWatcher.GetPairsCsvOnlyAsString());
            }

            if (_dirWatcher.GetXmlFilePathsWoPair().Count > 0)
            {
                InvokeAddOrChangeRange(XmlFiles, _dirWatcher.GetPairsXmlOnlyAsString());
            }

            if (_dirWatcher.GetDataPairsWithPair().Count > 0)
            {
                InvokeAddOrChangeRange(PairFiles, _dirWatcher.GetDataPairsWithPairAsString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _dirWatcher.Stop();
            CancelWatcher();
        }

        private void ButtonOut_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = Folders[1]
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (Folders[1] != dialog.SelectedPath)
            {
                Folders[1] = dialog.SelectedPath;
            }
        }

        private void UpdateWatcher(string dirFullPath)
        {
            CancelWatcher();
            _dirWatcher = new DirectoryWatcher(dirFullPath);
            _dirWatcher.PropertyChanged += (_, _) => UpdateUI();
            _dirWatcher.Start();
        }

        private void CancelWatcher()
        {
            _dirWatcher = null;
        }

        private void InvokeAddOrChangeRange(ObservableCollection<string> target, IList<string> source)
        {
            Dispatcher.Invoke(() => AddOrChangeRange(target, source));
        }

        private void AddOrChangeRange(ObservableCollection<string> target, IList<string> source)
        {
            if (source.Count < 1) return;
            for (int i = 0; i < source.Count; i++)
            {
                if (!target.Select(x => x.Substring(0, 40)).Contains(source[i].Substring(0, 40)))
                    target.Add(source[i]);
                else target[i] = source[i];
            }
        }

        private void ClearLists()
        {
            XmlFiles.Clear();
            CsvFiles.Clear();
            PairFiles.Clear();
        }

        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            TBox.Text = _dirWatcher.ProcessingOutputData(Folders[1]);
        }
    }
}