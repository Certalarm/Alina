using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity;
using AlinaLib.Domain.Entity.Base;
using System.IO;
using System.Text;

namespace AlinaLib.Data.Implementation
{
    internal class CsvFileReader : IFileReader
    {
        const string __userId = "UserId";
        const string __csvDelim = ";";

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
            return parts.Length < 4 
                ? emptyUser
                : new User(parts[0], parts[1], parts[2], parts[3]);
        }
    }
}
