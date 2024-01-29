using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class User
    {
        public string UserId { get; }
        public string Name { get; }
        public string SecondName { get; }
        public string Number { get; }


        #region .ctors
        public User(string userId, string name, string secondName, string number)
        {
            UserId = userId;
            Name = name;
            SecondName = secondName;
            Number = number;
        }
        #endregion
    }
}
