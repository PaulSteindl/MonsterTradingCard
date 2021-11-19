using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Enums;

namespace MonsterTradingCard.Abstract.Card
{
   abstract class Card
    {
        public readonly byte Dmg;
        public readonly string Name;
        public readonly Element Element;
        public readonly CardType CardType;

        protected Card(byte dmg, Element element, CardType cardType)
        {
            Dmg = dmg;
            Element = element;
            CardType = cardType;
            Name = element.ToString() + cardType.ToString();
        }
    }
}
