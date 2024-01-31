using AlinaLib.Data.Interface;
using AlinaLib.Domain.Entity;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AlinaLib.Data.Implementation
{
    internal class JsonWriter : IFileWriter
    {
        private string _fullPath;

        #region .ctors
        public JsonWriter(string fullPath)
        {
            _fullPath = fullPath;
        }
        #endregion

        public bool WriteAll(OutputData data)
        {
            string jsonText = getJson(data);
            if (jsonText.Length < 1) return false;
            return WriteToFile(_fullPath, jsonText);
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
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

        private bool WriteToFile(string fullPath, string data)
        {
            try
            {
                using StreamWriter writer = new(fullPath, Encoding.Unicode, getFileOptions());
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
                Mode = FileMode.CreateNew,
            };
    }
}
