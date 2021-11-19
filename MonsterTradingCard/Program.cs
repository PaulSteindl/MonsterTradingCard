using System;
using MonsterTradingCard.UserRelated.User;

namespace MonsterTradingCard
{
    class Program
    {
        static void Main(string[] args)
        {
            User user1 = new User();
            User user2 = new User();

            for(int i = 0; i < 5; i++)
                user1.BuyPackage();

            for (int i = 0; i < 4; i++)
                user2.BuyPackage();


        }
    }
}
