using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AlinaLib.Domain.Entity;
using System.IO;

namespace AlinaLib.Data.Implementation
{
    internal class JsonWriter : IFileWriter
    {
        public bool WriteAll(OutputData data, string fullPath)
        {
            string jsonText = getJson(data);
            if (jsonText.Length < 1) return false;
            return WriteToFile(jsonText, fullPath);
        }

        private string getJson(OutputData data)
        {
            string result;
            try
            {
                result = JsonSerializer.Serialize<OutputData>(data, getJsonOptions());
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        private JsonSerializerOptions getJsonOptions() =>
            new()
            {
                WriteIndented = true
            };

        private bool WriteToFile(string fullPath, string data)
        {
            try
            {
                using StreamWriter writer = new(fullPath, Encoding.UTF8, getFileOptions());
                writer.WriteLine(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private FileStreamOptions getFileOptions() =>
            new()
            {
                Access = FileAccess.Write,
                Mode = FileMode.CreateNew
            };
    }
}
