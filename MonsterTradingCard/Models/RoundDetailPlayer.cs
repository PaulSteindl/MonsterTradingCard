using MonsterTradingCard.Models.Enums.CardEffect;
using CARD = MonsterTradingCard.Models.Card;

namespace MonsterTradingCard.Models.RoundDetailPlayer
{
    public class RoundDetailPlayer
    {
        public string Username { get; set; }
        public CARD.Card Card { get; set; }
        public double ElementMultiplier { get; set; }
        public CardEffect CardEffect { get; set; }
        public double TotalDamage { get; set; }
        public int CardsLeft { get; set; }
    }
}
