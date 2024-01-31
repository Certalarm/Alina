using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlinaLib.Domain.Entity;
using static AlinaLib.Domain.UseCase.DirectoryWatcher.DirectoryWatcherHelper;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    public static class DirectoryWatcherExt
    {
        public static IList<DataPair> GetPairsXmlOnly(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .Where(x => !x.hasBothHalf() && x.XmlData != null)
                    .ToList()
                : Array.Empty<DataPair>();

        public static IList<DataPair> GetPairsCsvOnly(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .Where(x => !x.hasBothHalf() && x.CsvData != null)
                    .ToList()
            : Array.Empty<DataPair>();

        public static IList<string> GetCsvFilePathsWoPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.GetPairsCsvOnly()
                    .Select(x => x.CsvData!.Fullname)
                    .ToList()
                : Array.Empty<string>();

        public static IList<string> GetXmlFilePathsWoPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.GetPairsXmlOnly()
                    .Select(x => x.XmlData!.Fullname)
                    .ToList()
                : Array.Empty<string>();

        internal static IList<string> GetFilePaths(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .SelectMany(x => x.FullPaths())
                    .ToList()
                : Array.Empty<string>();

        public static IList<DataPair> GetDataPairsWoPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .Where(x => !x.hasBothHalf())
                    .ToList()
                : Array.Empty<DataPair>();

        public static IList<int> GetAllIndexesDataPairsWithPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? Enumerable.Range(0, watcher.DataPairs.Count)
                    .Where(x => watcher.DataPairs[x].hasBothHalf())
                    .ToList()
                : Array.Empty<int>();


        public static IList<int> GetNotCompletedIndexesDataPairsWithPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? Enumerable.Range(0, watcher.DataPairs.Count)
                    .Where(x => watcher.DataPairs[x].hasBothHalf() && watcher.DataPairs[x].isCompleted)
                    .ToList()
                : Array.Empty<int>();

        public static IList<int> GetProcessingIndexes(this DirectoryWatcher watcher, bool isCompletedInclude) =>
            isCompletedInclude
                ? watcher.GetNotCompletedIndexesDataPairsWithPair()
                : watcher.GetAllIndexesDataPairsWithPair();

        public static OutputData ToOutputData(this DataPair pair)
        {
            var records = GetRecords(pair);
            return new OutputData(records);
        }
    }
}
