using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Utility
{
    internal static class Txt
    {
        internal const string __csvExt = "csv";
        internal const string __xmlExt = "xml";
        internal const string __jsonExt = "json";
        internal const string __searchPattern = $"*.{__csvExt}|*.{__xmlExt}";

        internal const string __unknownFileType = "Неизвестный тип файла!";
        internal const string __dirNoExist = "Указанная папка не существует!";
        internal const string __dirNotRead = "Не могу прочитать содержимое папки!";
    }
}
