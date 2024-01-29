using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Data.Implementation
{
    internal class CsvFileReader : IFileReader
    {
        private string _fullPath = string.Empty;

        #region .ctors
        public CsvFileReader(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return;
            _fullPath = fullPath;
        }
        #endregion

        public IEnumerable<BaseEntity> ReadAll()
        {
            throw new NotImplementedException();
        }
    }
}
