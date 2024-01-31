using AlinaLib.Domain.Entity.Base;

namespace AlinaLib.Domain.Entity
{
    internal class Card: BaseEntity
    {
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
