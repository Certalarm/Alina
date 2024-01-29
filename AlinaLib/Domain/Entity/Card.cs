using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class Card
    {
        public readonly string UserId;
        public readonly string Pan;
        public readonly string ExpDate;


        #region .ctors
        public Card(string userId, string pan, string expDate)
        {
            UserId = userId;
            Pan = pan;
            ExpDate = expDate;
        }
        #endregion
    }
}
