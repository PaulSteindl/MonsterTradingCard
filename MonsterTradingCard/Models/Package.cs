using CARD = MonsterTradingCard.Models.Card;
using System.Collections.Generic;

namespace MonsterTradingCard.Models.Package
{
    public class Package
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public List<string> CardIds { get; set; }
    }
}
