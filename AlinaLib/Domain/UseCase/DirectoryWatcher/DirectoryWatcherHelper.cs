using AlinaLib.Domain.Entity;
using AlinaLib.Domain.Entity.Base;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    internal static class DirectoryWatcherHelper
    {
        public static IList<Record> GetRecords(DataPair pair)
        {
            if (pair.CsvData!.ItemCount() == 0) return Array.Empty<Record>();
            var result = new List<Record>(pair.CsvData.ItemCount());
            foreach (var entry in pair.CsvData.Items)
            {
                var pairIndex = GetPairIndex(entry.UserId, pair.XmlData!.Items);
                if (pairIndex < 0) continue;
                var record = ToRecord(pair.XmlData.Items[pairIndex] as Card, entry as User);
                if (!string.IsNullOrWhiteSpace(record.UserId))
                    result.Add(record);
            }
            return result;
        }

        private static int GetPairIndex(string csvUserId, List<BaseEntity> xmlList)
        {
            var indexes = Enumerable.Range(0, xmlList.Count)
                .Where(x => xmlList[x].UserId == csvUserId);
            return indexes.Any()
                ? indexes.First()
                : -1;
        }

        private static Record ToRecord(Card? card, User? user)
        {
            return new Record
            {
                UserId = card?.UserId ?? string.Empty,
                Pan = card?.Pan ?? string.Empty,
                ExpDate = card?.ExpDate ?? string.Empty,
                FirstName = user?.Name ?? string.Empty,
                LastName = user?.SecondName ?? string.Empty,
                Phone = user?.Number ?? string.Empty
            };
        }
    }
}
