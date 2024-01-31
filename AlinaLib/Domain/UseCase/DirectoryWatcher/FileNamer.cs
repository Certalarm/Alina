using System.IO;
using static AlinaLib.Utility.Txt;

namespace AlinaLib.Domain.UseCase.DirectoryWatcher
{
    internal static class FileNamer
    {
        public static string GetOutputJsonFilePath(string dirFullPath) =>
            string.IsNullOrWhiteSpace(dirFullPath)
                ? string.Empty
                : string.Concat(GetFullPathWoExt(dirFullPath), __jsonExt);

        private static string GetFullPathWoExt(string dirfFullPath) =>
            Path.Combine(dirfFullPath, GenerateFileNameWoExt());

        private static string GenerateFileNameWoExt()
        {
            var dt = DateTime.Now;
            var date = dt.ToString("yyy-MM-dd");
            var time = dt.ToString("HH-mm-ss-fff");
            return $"{date}_{time}";
        }
    }
}
