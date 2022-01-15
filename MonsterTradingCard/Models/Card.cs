using MonsterTradingCard.Enums;

namespace MonsterTradingCard.Models.Card
{
    public class Card
    {
        public string Name { get; set; }
        public int Dmg { get; set; }
        public Element Element { get; set; }
        public CardType CardType { get; set; }
    }
}
