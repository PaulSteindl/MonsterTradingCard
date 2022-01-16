//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MonsterTradingCard.Abstract.Card;
//using MonsterTradingCard.Enums;
//using MSCCE = MonsterTradingCard.Battle.CardEffect;
//using MonsterTradingCard.UserRelated.User;

//namespace MonsterTradingCard.Battle.Fight
//{
//    public class Fight
//    {
//        public Winner CardBattle(Card card1, Card card2)
//        {
//            MSCCE.CardEffect CardEffect = new MSCCE.CardEffect(card1, card2);
//            CardEffect.SpecialEffect();

//            if (card1.Dmg > card2.Dmg)
//            {
//                Console.WriteLine($"{card1.Name} gewinnt den Kampf!");
//                return Winner.PlayerOne;
//            }
//            else if(card1.Dmg < card2.Dmg)
//            {
//                Console.WriteLine($"{card2.Name} gewinnt den Kampf!");
//                return Winner.PlayerTwo;
//            }
//            else
//            {
//                Console.WriteLine($"Es ist ein Unentschieden!");
//                return Winner.Draw;
//            }
//        }

//        public bool CheckForWin(User user1, User user2, int rounds)
//        {
//            if (user2.Deck != null)
//            {
//                Console.WriteLine($"{user1.Username} gewinnt das Match, Glückwunsch!");
//                return true;
//            }
//            else if (user1.Deck != null)
//            {
//                Console.WriteLine($"{user2.Username} gewinnt das Match, Glückwunsch!");
//                return true;
//            }
//            else if (rounds > 100)
//            {
//                Console.WriteLine($"Es kommt zu einem Unentschieden!");
//                return true;
//            }

//            return false;
//        }


//    }
//}
