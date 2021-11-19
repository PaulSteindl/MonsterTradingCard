using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.Enums;

namespace MonsterTradingCard.CardRelated.SpellCard
{
    class SpellCard : Card
    {
        public SpellCard(byte dmg, Element element, CardType cardType) : base(dmg, element, cardType) { }
    }
}
