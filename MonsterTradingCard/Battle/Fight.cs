using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.Battle;
using MonsterTradingCard.Enums

namespace MonsterTradingCard.Battle
{
    class Fight
    {
        private CardEffect CardEffect;

        public Winner CardBattle(Card Card1, Card Card2)
        {
            CardEffect.SpecialEffect(Card1, Card2);
        }
    }
}
