using DECK = MonsterTradingCard.Models.Deck;
using ROUND = MonsterTradingCard.Models.Round;
using System.Collections.Generic;

namespace MonsterTradingCard.Models.FightLog
{
    class FightLog
    {
        public string Winner { get; set; }
        public string Loser { get; set; }
        public bool Draw { get; set; }
        public List<DECK.Deck> Decks { get; set; }
        public int RoundCount { get; set; }
        public List<ROUND.Round> Rounds { get; set; }
    }
}
