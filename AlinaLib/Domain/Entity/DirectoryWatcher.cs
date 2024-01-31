using AlinaLib.Data.Implementation;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using static AlinaLib.Utility.Txt;
using static AlinaLib.Domain.Entity.DirectoryWatcherHelper;
using System;

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

        public string ProcessingOutputData(string outputDirPath, bool isCompletedInclude = false)
        {
            var pairIndexes = this.GetProcessingIndexes(isCompletedInclude);
            if (!pairIndexes.Any()) return string.Empty;
            string processedFileNames = string.Empty;
            foreach(var index in  pairIndexes)
            {
                var savedName = SaveOutputData(outputDirPath, index);
                if(savedName.Length > 0)
                    processedFileNames += savedName;
            }
            return processedFileNames.Length > 0
                ? processedFileNames.Substring(0, processedFileNames.Length - Environment.NewLine.Length)
                : processedFileNames;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string SaveOutputData(string outputDirPath, int index)
        {
            var processedFileName = string.Empty;
            var fullPath = FileNamer.GetOutputJsonFilePath(outputDirPath);
            var jsonWriter = new JsonWriter(fullPath);
            if (jsonWriter.WriteAll(DataPairs[index].ToOutputData()))
            {
                processedFileName += string.Concat(Path.GetFileName(fullPath), Environment.NewLine);
                DataPairs[index].isCompleted = true;
            }
            return processedFileName;
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
