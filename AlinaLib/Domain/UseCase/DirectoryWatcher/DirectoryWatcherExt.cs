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
                    .Where(x => !x.HasBothHalf() && x.XmlData != null)
                    .ToList()
                : Array.Empty<DataPair>();

        public static IList<DataPair> GetPairsCsvOnly(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .Where(x => !x.HasBothHalf() && x.CsvData != null)
                    .ToList()
            : Array.Empty<DataPair>();

        public static IList<string> GetPairsCsvOnlyAsString(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.GetPairsCsvOnly()
                    .Select(x => $"{x.CsvData!.Name}, размер: {x.CsvData.SizeInB} Байт, дата изменения {x.CsvData.LastModifiedUtc}")
                    .ToList()
            : Array.Empty<string>();

        public static IList<string> GetPairsXmlOnlyAsString(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.GetPairsXmlOnly()
                    .Select(x => $"{x.XmlData!.Name}, размер: {x.XmlData.SizeInB} Байт, дата изменения {x.XmlData.LastModifiedUtc}")
                    .ToList()
            : Array.Empty<string>();

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
                    .Where(x => !x.HasBothHalf())
                    .ToList()
                : Array.Empty<DataPair>();

        public static IList<DataPair> GetDataPairsWithPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.DataPairs
                    .Where(x => x.HasBothHalf())
                    .ToList()
                : Array.Empty<DataPair>();

        public static IList<string> GetDataPairsWithPairAsString(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? watcher.GetDataPairsWithPair()
                    .Select(x => 
                        $"Пара: {x.CsvData!.Name} + {x.XmlData!.Name}, количество записей: {x.RecordCount}, статус: {StatusToText(x.isCompleted)}")
                    .ToList()
                : Array.Empty<string>();

        public static IList<int> GetAllIndexesDataPairsWithPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? Enumerable.Range(0, watcher.DataPairs.Count)
                    .Where(x => watcher.DataPairs[x].HasBothHalf())
                    .ToList()
                : Array.Empty<int>();


        public static IList<int> GetNotCompletedIndexesDataPairsWithPair(this DirectoryWatcher watcher) =>
            watcher.DataPairs.Any()
                ? Enumerable.Range(0, watcher.DataPairs.Count)
                    .Where(x => watcher.DataPairs[x].HasBothHalf() && watcher.DataPairs[x].isCompleted)
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

        private static string StatusToText(bool isCompleted) =>
            isCompleted
                ? "обработана"
                : "не обработана";
    }
}
