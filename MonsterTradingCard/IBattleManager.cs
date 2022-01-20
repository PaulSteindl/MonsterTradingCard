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
        /// <summary>
        /// Startet Kampf und wickelt ihn ab
        /// </summary>
        FightLog Startbattle();
        /// <summary>
        /// Setzt Gewinner und Verlierer bzw Draw für Runden
        /// </summary>
        void SetWinnerLoserDraw(RoundDetailPlayer player, RoundDetailPlayer opponent, Round round);
        /// <summary>
        /// Berechnet ElementMultiplikator, List[0] für playerElement List[1] für opponentElemnt
        /// </summary>
        List<float> ElementMultCalc(Element playerElement, Element opponentElement);
        /// <summary>
        /// Schaut ob Karten Kombination eine CardEffect hat und liefert diesen für beide karten die Effekte zurück, Array[0] player Array[1] opponent
        /// </summary>
        CardEffect[] CheckCardEffect(Card playerCard, Card opponentCard);
        /// <summary>
        /// Holt Card Liste anhand von Deck
        /// </summary>
        List<Card> SetupCards(Deck deck);
    }
}