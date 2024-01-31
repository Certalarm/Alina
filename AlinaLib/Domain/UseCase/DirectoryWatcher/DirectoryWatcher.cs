using AlinaLib.Data.Implementation;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    public class DirectoryWatcher : INotifyPropertyChanged
    {
        private FileSystemWatcher _watcher;
        private BlockingCollection<string> _filteredFilePathsQueue;
        private List<DataPair> _dataPairs = new List<DataPair>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<DataPair> DataPairs
        {
            get { return _dataPairs; }
            private set
            {
                _dataPairs = value;
                OnPropertyChanged(nameof(DataPairs));
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
            foreach (var index in pairIndexes)
            {
                var savedName = SaveOutputData(outputDirPath, index);
                if (savedName.Length > 0)
                    processedFileNames += savedName;
            }
            return processedFileNames.Length > 0
                ? processedFileNames[..^Environment.NewLine.Length]
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
                OnPropertyChanged(nameof(DataPairs));
            }
            return processedFileName;
        }

        private void InitWatcherParams(string dirFullPath)
        {
            if (IsBadParam(dirFullPath))
                throw new ArgumentException(__dirNoExist);
            _watcher.Path = dirFullPath;
            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.Filter = "*.*";
            _watcher.Created += (_, e) => StartProcessingFileChanges(e.FullPath);
        }

        private void StartProcessingFileChanges(string fullPath)
        {
            if (isBadExt(fullPath)) return;
            _filteredFilePathsQueue.Add(fullPath);
            ProcessingFileChanges();
        }

        private bool isBadExt(string fullPath)
        {
            var needExt = new string[] { __csvExt, __xmlExt };
            return !needExt
                .Any(x => fullPath.ToLower().EndsWith(x));
        }

        private bool IsBadParam(string dirFullPath)
        {
            if (string.IsNullOrWhiteSpace(dirFullPath)) return true;
            return !Directory.Exists(dirFullPath);
        }

        private void AddExistFilenamesToQueue()
        {
            foreach (var filename in getExistFiles())
            {
                _filteredFilePathsQueue.Add(filename);
            }
        }

        private IEnumerable<string> getExistFiles()
        {
            try
            {
                return Directory.EnumerateFiles(_watcher.Path, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x=> x.ToLower().EndsWith(__csvExt) || x.ToLower().EndsWith(__xmlExt));
            }
            catch
            {
                throw new Exception(__dirNotRead);
            }
        }

        private void ProcessingFileChanges()
        {
            if (_filteredFilePathsQueue.Count < 1) return;
            while (!_filteredFilePathsQueue.IsCompleted)
            {
                if (!_filteredFilePathsQueue.Any()) break;
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
                    OnPropertyChanged(nameof(DataPairs));
                    break;
                }
            }
            if (!wasFindedPair)
            {
                DataPairs.Add(new DataPair(filePath));
                OnPropertyChanged(nameof(DataPairs));
            }

        }
    }
}
