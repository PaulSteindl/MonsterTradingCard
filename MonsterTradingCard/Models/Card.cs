namespace MonsterTradingCard.Models.Card
{
    public class Card
    {
        public override string ToString() { return $"Id: {Id}, Name: {Name}, Damage: {Damage}; "; }
        public string Id { get; set; } 
        public string Name { get; set; }
        public double Damage { get; set; }
    }
}
