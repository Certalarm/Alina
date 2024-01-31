using AlinaLib.Domain.Entity;
using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Data.Interface
{
    internal interface IFileWriter
    {
        public bool WriteAll(OutputData data);
    }
}
