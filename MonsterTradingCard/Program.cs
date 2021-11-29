using System;
using MonsterTradingCard.UserRelated.User;
using MonsterTradingCard.Battle.CardEffect;
using MonsterTradingCard.CardRelated.MonsterCard;
using MonsterTradingCard.CardRelated.SpellCard;


//Stripe - paymant gateway
namespace MonsterTradingCard
{
    class Program
    {
        static void Main(string[] args)
        {
            //User user1 = new User();
            //User user2 = new User();

            //for(int i = 0; i < 5; i++)
            //    user1.BuyPackage();

            //for (int i = 0; i < 4; i++)
            //    user2.BuyPackage();

            MonsterCard testCardMonster = new MonsterCard(20, Enums.Element.Normal, Enums.CardType.Knight);
            SpellCard testCardSpell = new SpellCard(10, Enums.Element.Water);
            CardEffect test = new CardEffect(testCardMonster, testCardSpell);
            test.SpecialEffect();

            if (testCardMonster.Dmg > testCardSpell.Dmg)
                Console.WriteLine($"{testCardMonster.Name} gewinnt");
            else if(testCardSpell.Dmg > testCardMonster.Dmg)
                Console.WriteLine($"{testCardSpell.Name} gewinnt");
            else
                Console.WriteLine("Draw");
        }
    }
}
