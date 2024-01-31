using AlinaLib.Domain.Entity.Base;

namespace AlinaLib.Domain.Entity
{
    internal class User: BaseEntity
    {
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
