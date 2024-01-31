using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    public class DataPair
    {
        internal FileData? XmlData { get; private set; }
        internal FileData? CsvData { get; private set; }

        public int RecordCount { get; private set; } = 0;
        public bool isCompleted { get; set; } = false;

        #region .ctors
        public DataPair(string fullPathAnyHalf)
        {
            InitHalf(fullPathAnyHalf);
        }
        #endregion

        public bool HasBothHalf() => XmlData != null && CsvData != null;

        public bool FindHalf(string fullPath)
        {
            //foreach (var path in fullPaths)
            //{
            var firstHalfExt = GetExistHalfExtension();
            var secondHalf = new FileData(fullPath);
            if (!IsGoodExts(firstHalfExt, secondHalf.Extension)) return false;
            var existHalf = GetExistHalf(firstHalfExt);
            ReadItems(existHalf, secondHalf);
            if (IsSkipPath(existHalf, secondHalf)) return false;
            updateHalfs(existHalf, secondHalf);
            return true;
            //}
            // return false;
        }

        public IList<string> FullPaths()
        {
            var result = new List<string>();
            if (CsvData != null)
                result.Add(CsvData.Fullname);
            if (XmlData != null)
                result.Add(XmlData.Fullname);
            return result;
        }

        private void InitHalf(string fullPathAnyHalf)
        {
            var someFileData = new FileData(fullPathAnyHalf);
            switch (someFileData.Extension)
            {
                case __csvExt: CsvData = new FileData(someFileData); break;
                case __xmlExt: XmlData = new FileData(someFileData); break;
                default: throw new ArgumentException(__unknownFileType);
            }
        }

        private string GetExistHalfExtension()
        {
            var ext = CsvData?.Extension ?? string.Empty;
            if (ext.Length > 0) return ext;
            ext = XmlData?.Extension ?? string.Empty;
            return ext;
        }

        private void ReadItems(FileData existHalf, FileData secondHalf)
        {
            if (existHalf!.ItemCount() == 0)
                existHalf.ReadItems();
            secondHalf.ReadItems();
        }

        private bool IsGoodExts(string firstExt, string secondExt) =>
            secondExt.Length > 0 && firstExt.Length > 0 && secondExt != firstExt;

        private FileData GetExistHalf(string ext) =>
            ext == __csvExt
                ? CsvData!
                : XmlData!;

        private bool IsSkipPath(FileData existHalf, FileData secondHalf)
        {
            if (existHalf.ItemCount() == 0 || secondHalf.ItemCount() == 0) return true;
            if (existHalf.ItemCount() != secondHalf.ItemCount()) return true;
            return !IsEqualsUserIds(existHalf.GetUserIds(), secondHalf.GetUserIds());
        }

        private void updateHalfs(FileData existHalf, FileData secondHalf)
        {
            existHalf.HasPair = true;
            secondHalf.HasPair = true;
            if (existHalf.Extension == __csvExt)
                XmlData = new FileData(secondHalf);
            if (existHalf.Extension == __xmlExt)
                CsvData = new FileData(secondHalf);
            RecordCount = existHalf.ItemCount();
        }

        private bool IsEqualsUserIds(IEnumerable<string> first, IEnumerable<string> second) =>
            !first
                .Except(second)
                .Any();
    }
}
