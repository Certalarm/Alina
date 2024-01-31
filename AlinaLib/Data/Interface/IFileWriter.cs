using AlinaLib.Domain.Entity;

namespace AlinaLib.Data.Interface
{
    internal interface IFileWriter
    {
        public bool WriteAll(OutputData data);
    }
}
