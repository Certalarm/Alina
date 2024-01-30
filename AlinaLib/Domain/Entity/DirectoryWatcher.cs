using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.Entity
{
    internal class DirectoryWatcher: INotifyPropertyChanged
    {
        private FileSystemWatcher _watcher;
        private BlockingCollection<string> _filteredFilePathsQueue;
        private List<DataPair> _dataPairs;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<DataPair> DataPairs 
        {
            get { return _dataPairs; } 
            private set
            {
                _dataPairs = value;
                OnPropertyChanged(/*nameof(DataPairs)*/);
            }
        }

        #region .ctors
        public DirectoryWatcher(string dirFullPath)
        {
            _watcher = new FileSystemWatcher();
            _filteredFilePathsQueue = new BlockingCollection<string>();
            _dataPairs = new List<DataPair>();
            InitWatcherParams(dirFullPath);     
        }
        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitWatcherParams(string dirFullPath)
        {
            if (IsBadParam(dirFullPath))
                throw new ArgumentException(__dirNoExist);
            _watcher.Path = dirFullPath;
            _watcher.Filter = $"*.{__csvExt} | *.{__xmlExt}";
            _watcher.Changed += (_, e) => _filteredFilePathsQueue.Add(e.FullPath);
            _watcher.Created += (_, e) => _filteredFilePathsQueue.Add(e.FullPath);
        }

        public IList<DataPair> GetPairsCsvOnly() =>
            DataPairs.Any()
                ? DataPairs
                    .Where(x => !x.hasBothHalf() && x.CsvData != null)
                    .ToList()
                : Array.Empty<DataPair>();

        public IList<DataPair> GetPairsXmlOnly() =>
            DataPairs.Any()
                ? DataPairs
                    .Where(x => !x.hasBothHalf() && x.XmlData != null)
                    .ToList()
                : Array.Empty<DataPair>();

        public IList<string> GetCsvFilePathsWoPair() =>
            DataPairs.Any()
                ? GetPairsCsvOnly()
                    .Select(x => x.CsvData!.Fullname)
                    .ToList()
                : Array.Empty<string>();

        public IList<string> GetXmlFilePathsWoPair() =>
            DataPairs.Any()
                ? GetPairsXmlOnly()
                    .Select(x => x.XmlData!.Fullname)
                    .ToList()
                : Array.Empty<string>();

        public IList<string> getFilePaths() =>
            DataPairs.Any()
                ? DataPairs
                    .SelectMany(x => x.FullPaths())
                    .ToList()
                : Array.Empty<string>();

        public IList<DataPair> GetDataPairsWoPair() =>
            DataPairs.Any()
                ? DataPairs
                    .Where(x => !x.hasBothHalf())
                    .ToList()
                : Array.Empty<DataPair>();

        public void Start()
        {
            if (_watcher.EnableRaisingEvents) return;
            _watcher.EnableRaisingEvents = true;
            ProcessingFileChanges();
        }

        public void Stop()
        {
            if (!_watcher.EnableRaisingEvents) return;
            _watcher.EnableRaisingEvents = false;
        }

        private bool IsBadParam(string dirFullPath)
        {
            if (string.IsNullOrWhiteSpace(dirFullPath)) return true;
            return !Directory.Exists(dirFullPath);
        }

        //private void Watcher_Change(object sender, FileSystemEventArgs e)
        //{
        //   // e.FullPath;
        //}

        private void ProcessingFileChanges()
        {
            while(!_filteredFilePathsQueue.IsCompleted)
            {
                updateDataPairs(_filteredFilePathsQueue.Take());
            }
        }

        private void updateDataPairs(string filePath)
        {
            if (!DataPairs.Any())
            {
                DataPairs.Add(new DataPair(filePath));
                // change Event!!!!
            }
            else
            {
                if (getFilePaths().Contains(filePath)) return;
                FindHalfOrAdd(filePath);
                // change Event!!!
            }
        }

        private void FindHalfOrAdd(string filePath)
        {
            bool wasFindedPair = false;
            foreach (var dataPair in GetDataPairsWoPair())
            {
                if (dataPair.FindHalf(filePath))
                {
                    wasFindedPair = true;
                    break;
                }
            }
            if (!wasFindedPair)
                DataPairs.Add(new DataPair(filePath));
        }
    }
}
