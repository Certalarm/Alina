using AlinaLib;
using AlinaLib.Domain.Entity;
using AlinaLib.Domain.UseCase.DirectoryWatcher;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
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
                InvokeAddRange(CsvFiles, _dirWatcher.GetPairsCsvOnlyAsString());
            }

            if (_dirWatcher.GetXmlFilePathsWoPair().Count > 0)
            {
                InvokeAddRange(XmlFiles, _dirWatcher.GetPairsXmlOnlyAsString());
            }

            if (_dirWatcher.GetDataPairsWithPair().Count > 0)
            {
                InvokeAddRange(PairFiles, _dirWatcher.GetDataPairsWithPairAsString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CancelWatcher();
        }

        private void ButtonOut_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = Folders[1]
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Folders[1] = dialog.SelectedPath;
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

        private void InvokeAddRange(ObservableCollection<string> target, IList<string> source)
        {
            Dispatcher.Invoke(() => AddRange(target, source));
        }

        private void AddRange(ObservableCollection<string> target, IList<string> source)
        {
            if (source.Count < 1) return;
            foreach (var entry in source)
            {
                if (!target.Contains(entry))
                    target.Add(entry);
            }
        }

        private void ClearLists()
        {
            XmlFiles.Clear();
            CsvFiles.Clear();
            PairFiles.Clear();
        }
    }
}