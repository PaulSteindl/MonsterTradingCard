using MonsterTradingCard.Models.FightLog;
using MonsterTradingCard.Models.RoundDetailPlayer;
using MonsterTradingCard.Models.Round;
using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.Enums.CardEffect;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.Models.Deck;
using System.Collections.Generic;

namespace MonsterTradingCard.IBattleManager
{
    public interface IBattleManager
    {
        FightLog Startbattle();
        void SetWinnerLoserDraw(RoundDetailPlayer player, RoundDetailPlayer opponent, Round round);
        List<float> ElementMultCalc(Element playerElement, Element opponentElement);
        CardEffect[] CheckCardEffect(Card playerCard, Card opponentCard);
        List<Card> SetupCards(Deck deck);
    }
}