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
            Folders[0] = dialog.SelectedPath;
            UpdateWatcher(Folders[0]);
        }

        private void UpdateUI()
        {
            if (_dirWatcher.GetCsvFilePathsWoPair().Count > 0)
            {
                AddRange(CsvFiles, _dirWatcher.GetPairsCsvOnlyAsString());
            }

            if (_dirWatcher.GetXmlFilePathsWoPair().Count > 0)
            {
                AddRange(XmlFiles, _dirWatcher.GetPairsXmlOnlyAsString());
            }

            if(_dirWatcher.GetDataPairsWithPair().Count > 0)
            {
                AddRange(PairFiles, _dirWatcher.GetDataPairsWithPairAsString());
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
            UpdateUI();
            //if (_dirWatcher.DataPairs.Count > 0)
            //    System.Windows.Forms.MessageBox.Show("la: " + _dirWatcher.GetCsvFilePathsWoPair()[0]);
        }

        private void CancelWatcher()
        {
            // _dirWatcher.PropertyChanged -= (_, _) => UdateUI();
            //_dirWatcher.Stop();
            _dirWatcher = null;
        }

        private void AddRange(ObservableCollection<string> target, IList<string> source)
        {
            foreach (var entry in source)
                target.Add(entry);
        }
    }
}