using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.Entity
{
    public class DirectoryWatcher: INotifyPropertyChanged
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

        public void Start()
        {
            if (_watcher.EnableRaisingEvents) return;
            AddExistFilenamesToQueue();
            _watcher.EnableRaisingEvents = true;
            ProcessingFileChanges();
        }

        public void Stop()
        {
            if (!_watcher.EnableRaisingEvents) return;
            _watcher.EnableRaisingEvents = false;
        }

        public string ProcessingOutputData(bool isCompletedInclude)
        {
            var pairIndexes = isCompletedInclude
                ? Enumerable.Range(0, DataPairs.Count)
                : this.GetIndexesDataPairsWithPair();

        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitWatcherParams(string dirFullPath)
        {
            if (IsBadParam(dirFullPath))
                throw new ArgumentException(__dirNoExist);
            _watcher.Path = dirFullPath;
            _watcher.Filter = __searchPattern;
            _watcher.Changed += (_, e) => StartProcessingFileChanges(e.FullPath);
            _watcher.Created += (_, e) => StartProcessingFileChanges(e.FullPath);
        }

        private void StartProcessingFileChanges(string fullPath)
        {
            _filteredFilePathsQueue.Add(fullPath);
            ProcessingFileChanges();
        }

        private bool IsBadParam(string dirFullPath)
        {
            if (string.IsNullOrWhiteSpace(dirFullPath)) return true;
            return !Directory.Exists(dirFullPath);
        }

        private void AddExistFilenamesToQueue()
        {
            foreach(var filename in getExistFiles())
            {
                _filteredFilePathsQueue.Add(filename);
            }
        }

        private IEnumerable<string> getExistFiles()
        {
            try
            {
                return Directory.EnumerateFiles(_watcher.Path, __searchPattern, SearchOption.TopDirectoryOnly);
            }
            catch
            {
                throw new Exception(__dirNotRead);
            }
        }

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
            }
            else
            {
                if (this.GetFilePaths().Contains(filePath)) return;
                FindHalfOrAdd(filePath);
            }
        }

        private void FindHalfOrAdd(string filePath)
        {
            bool wasFindedPair = false;
            foreach (var dataPair in this.GetDataPairsWoPair())
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
