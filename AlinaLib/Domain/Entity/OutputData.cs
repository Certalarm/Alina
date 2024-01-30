using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    public class OutputData
    {
        public List<Record> Records { get; set; }

        #region .ctors

        public OutputData(IEnumerable<Record> records)
        {
            Records = new List<Record>(records);
        }
        #endregion
    }
}
