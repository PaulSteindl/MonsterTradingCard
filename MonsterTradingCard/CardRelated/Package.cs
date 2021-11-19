using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.Enums;
using MonsterTradingCard.CardRelated.MonsterCard;

namespace MonsterTradingCard.CardRelated.Package
{
    class Package
    {
        byte size = 5;
        byte choose = 4;

        public List<Card> CardList { get; }
        Package()
        {
            for(int i = 0; i < size; i++)
            {
                Random ran = new Random();

                switch(ran.Next())
                {
                    case 0:
                        break;

                    case 1:
                        MonsterCard card = new MonsterCard();
                        break;

                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
