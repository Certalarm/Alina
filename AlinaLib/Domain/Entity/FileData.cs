using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class FileData
    {
        private FileMetaInfo _metaInfo;

        public string Name { get; private set; } = string.Empty;
        public string Extension { get; private set; } = string.Empty;

        public List<BaseEntity> Items {  get; private set; } = new List<BaseEntity>();

        public bool isCompleted { get; private set; } = false;

        public bool hasPair { get; private set; } = false;

        #region .ctors
        public FileData(string fullPath)
        {
            _metaInfo = new FileMetaInfo(fullPath);
            init();
        }
        #endregion

        private void init()
        {
            Name = _metaInfo.Filename;
            Extension = _metaInfo.Extension;
        }

        public int ItemCount() => Items.Count;

        public void AddItems(IEnumerable<BaseEntity> items)
        {
            if (!items.Any()) return;
            Items.AddRange(items);
        }
    }
}
