using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity.Base
{
    internal abstract class BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
    }
}
