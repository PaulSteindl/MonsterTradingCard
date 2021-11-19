using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.UserRelated.User;
using MonsterTradingCard.CardRelated.Package;

namespace MonsterTradingCard.StoreRelated.Store
{
    class Store
    {
        private byte price = 5;
        public void BuyPackage(User user)
        {
            if(user.Coins > price)
            {
                Package package = new Package();
                package.
            }
        }
    }
}
