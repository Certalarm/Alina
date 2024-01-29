using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class FileMetaInfo
    {
        private FileInfo? _fileInfo;

        public string FullPath { get; private set; } = string.Empty;
        public string Filename { get; private set; } = string.Empty;
        public string Extension { get; private set; } = string.Empty;
        public string LastModifiedUtc { get; private set; } = string.Empty;
        public string SizeInB { get; private set; } = string.Empty;

        #region .ctors
        public FileMetaInfo(string fullPath)
        {
            if (isBadParam(fullPath)) return;
            init(fullPath);
        }
        #endregion

        private bool isBadParam(string fullPath) =>
            string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath);

        private bool tryInitFileInfo(string fullPath)
        {
            try
            {
                _fileInfo = new FileInfo(fullPath);
            }
            catch
            {
                return false;
            }
            return _fileInfo != null;
        } 

        private void init(string fullPath)
        {
            FullPath = fullPath;
            initFilename(fullPath);
            initExtension(fullPath);
            if (!tryInitFileInfo(fullPath)) return;
            initLastModified();
            initSizeInB();
        }

        private void initFilename(string fullPath) =>  Filename = Path.GetFileName(fullPath);
        
        private void initExtension(string fullPath) => Extension = Path.GetExtension(fullPath);

        private void initLastModified() => LastModifiedUtc = _fileInfo!.LastWriteTimeUtc.ToString();
        
        private void initSizeInB() => SizeInB = _fileInfo!.Length.ToString();
    }
}
