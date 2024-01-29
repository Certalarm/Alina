using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class FilePair
    {
        public FileMetaInfo XmlInfo { get; private set; }
        public FileMetaInfo CsvInfo { get; private set; }

        #region .ctors
        public FilePair()
        {
            
        }
        #endregion
    }
}
