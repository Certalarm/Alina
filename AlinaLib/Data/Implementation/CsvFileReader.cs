using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity;
using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Shapes;

namespace AlinaLib.Data.Implementation
{
    internal class CsvFileReader : IFileReader
    {
        private static readonly string __userId = "UserId";
        private static readonly string __csvDelim = ";";

        private readonly string _fullPath = string.Empty;

        #region .ctors
        public CsvFileReader(string fullPath)
        {
            _fullPath = fullPath;
        }
        #endregion

        public IEnumerable<BaseEntity> ReadAll()
        {
            var result = new List<BaseEntity>();
            if (string.IsNullOrWhiteSpace(_fullPath)) return result;
            foreach(var line in ReadLines())
            {
                if (line.StartsWith(__userId)) continue;
                var user = ParseUser(line);
                if (user.UserId.Length > 0)
                    result.Add(user);
            }
            return result;
        }

        private IEnumerable<string> ReadLines()
        {
            using var fileReader = new StreamReader(_fullPath, Encoding.UTF8);
            string line;
            while ((line = fileReader.ReadLine()!) != null)
            {
                yield return line;
            }
        }

        private User ParseUser(string line)
        {
            var emptyUser = new User(string.Empty, string.Empty, string.Empty, string.Empty);
            var parts = line.Split(__csvDelim);
            if (parts.Length < 4) return emptyUser;
            return new User(parts[0], parts[1], parts[2], parts[3]);
        }
    }
}
