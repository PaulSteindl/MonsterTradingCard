using MonsterTradingCard.Models.Enums.CardType;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.Models.Enums.Species;

namespace MonsterTradingCard.Models.TradingDeal
{
    public class TradingDeal
    {
        public string Id { get; set; }
        public string Usertoken { get; set; }
        public string CardToTrade { get; set; }
        public int? MinimumDamage { get; set; }
        public Element? Element { get; set; }
        public CardType? Type { get; set; }
        public Species? Species { get; set; }
    }
}
