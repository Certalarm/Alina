using AlinaLib.Domain.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlinaLib.Domain.Entity
{
    internal class CardUser: BaseEntity
    {
        public string Pan { get; private set; } = string.Empty;
        public string ExpDate { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        #region .ctors
        public CardUser(Card card, User user) 
        {
            if (isBadParams(card, user)) return;
            init(card, user);
        }
        #endregion

        private void init(Card card, User user)
        {
            UserId = card.UserId;
            Pan = card.Pan;
            ExpDate = card.ExpDate;
            FirstName = user.Name;
            LastName = user.SecondName;
            Phone = user.Number;
        }

        private bool isBadParams(Card card, User user) =>
            isUserIdEmpty(card, user) || isUserIdNotEquals(card, user);

        private bool isUserIdEmpty(Card card, User user) => 
            string.IsNullOrWhiteSpace(card.UserId) || string.IsNullOrWhiteSpace(user.UserId);

        private bool isUserIdNotEquals(Card card, User user) =>
            !string.Equals(card.UserId, user.UserId, StringComparison.OrdinalIgnoreCase);
    }
}
