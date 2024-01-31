using AlinaLib.Domain.Entity.Base;

namespace AlinaLib.Data.Interface
{
    internal interface IFileReader
    {
        public IEnumerable<BaseEntity> ReadAll();
    }
}
