﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.Entity
{
    internal class DataPair
    {
        public FileData? XmlData { get; private set; }
        public FileData? CsvData { get; private set; }

        public int RecordCount { get; private set; } = 0;
        public bool isCompleted { get; private set; } = false;

        #region .ctors
        public DataPair(string fullPathAnyHalf)
        {
            InitHalf(fullPathAnyHalf);       
        }
        #endregion

        public bool hasBothHalf() => XmlData != null && CsvData != null;

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

        public bool FindHalf(IEnumerable<string> fullPaths)
        {
            foreach (var path in fullPaths)
            {
                var firstHalfExt = GetExistHalfExtension();
                var secondHalf = new FileData(path);
                if (!IsGoodExts(firstHalfExt, secondHalf.Extension)) continue;
                var existHalf = GetExistHalf(firstHalfExt);
                ReadItems(existHalf, secondHalf);
                if (IsSkipPath(existHalf, secondHalf)) continue;
                updateHalfs(existHalf, secondHalf);
                return true;
            }
            return false;
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
        }

        private bool IsEqualsUserIds(IEnumerable<string> first, IEnumerable<string> second) =>
            !first
                .Except(second)
                .Any();
    }
}