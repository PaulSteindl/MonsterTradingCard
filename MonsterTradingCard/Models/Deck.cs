using System.Collections.Generic;

namespace MonsterTradingCard.Models.Deck
{
    public class Deck
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public List<string> CardIds { get; set; }
    }
}
