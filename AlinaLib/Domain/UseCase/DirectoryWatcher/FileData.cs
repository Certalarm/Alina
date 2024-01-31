using AlinaLib.Data.Implementation;
using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    internal class FileData
    {
        private FileMetaInfo _metaInfo;

        public string Name { get; private set; } = string.Empty;
        public string Fullname { get; private set; } = string.Empty;
        public string Extension { get; private set; } = string.Empty;
        public string LastModifiedUtc { get; private set; } = string.Empty;
        public string SizeInB { get; set; } = string.Empty;

        public List<BaseEntity> Items { get; private set; } = new List<BaseEntity>();

        public bool IsCompleted { get; set; } = false;

        public bool HasPair { get; set; } = false;

        #region .ctors
        public FileData(string fullPath)
        {
            _metaInfo = new FileMetaInfo(fullPath);
            Init();
        }

        public FileData(FileData fileData)
        {
            _metaInfo = new FileMetaInfo(fileData._metaInfo.FullPath);
            Fullname = fileData._metaInfo.FullPath;
            Name = fileData.Name;
            Extension = fileData.Extension;
            LastModifiedUtc = fileData.LastModifiedUtc;
            SizeInB = fileData.SizeInB;
            Items = fileData.Items;
            IsCompleted = fileData.IsCompleted;
            HasPair = fileData.HasPair;
        }
        #endregion

        private void Init()
        {
            Fullname = _metaInfo.FullPath;
            Name = _metaInfo.Filename;
            Extension = _metaInfo.Extension;
            LastModifiedUtc = _metaInfo.LastModifiedUtc;
            SizeInB = _metaInfo.SizeInB;
        }

        public int ItemCount() => Items.Count;

        public IReadOnlyList<string> GetUserIds() =>
            Items.Any()
                ? Items
                    .Select(x => x.UserId)
                    .ToList()
                : new List<string>();

        public void ReadItems()
        {
            IFileReader reader = СreateReader();
            Items.AddRange(reader.ReadAll());
        }

        private IFileReader СreateReader() =>
            Extension.ToLower() switch
            {
                __xmlExt => new XmlFileReader(_metaInfo.FullPath),
                __csvExt => new CsvFileReader(_metaInfo.FullPath),
                _ => throw new ArgumentException(__unknownFileType)
            };
    }
}
