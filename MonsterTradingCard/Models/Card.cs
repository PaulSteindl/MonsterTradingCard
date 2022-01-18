using MonsterTradingCard.Models.Enums.CardType;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.Models.Enums.Species;
using System.Collections.Generic;

namespace MonsterTradingCard.Models.Card
{

    public class Card
    {
        static Dictionary<string, Element> NameToElement = new Dictionary<string, Element>(){
            {"WaterSpell", Element.water},
            {"WaterGoblin", Element.water},
            {"Kraken", Element.water},
            {"RegularSpell", Element.normal},
            {"Knight", Element.normal},
            {"Wizzard", Element.normal},
            {"Ork", Element.normal},
            {"FireSpell", Element.fire},
            {"Dragon", Element.fire},
            {"FireElf", Element.fire}
        };

        static Dictionary<string, CardType> NameToType = new Dictionary<string, CardType>(){
            {"WaterSpell", CardType.spell},
            {"RegularSpell", CardType.spell},
            {"FireSpell", CardType.spell},
            {"WaterGoblin", CardType.monster},
            {"Kraken", CardType.monster},
            {"Knight", CardType.monster},
            {"Wizzard", CardType.monster},
            {"Ork", CardType.monster},
            {"Dragon", CardType.monster},
            {"FireElf", CardType.monster}
        };

        static Dictionary<string, Species> NameToSpecies = new Dictionary<string, Species>(){
            {"WaterSpell", Species.none},
            {"RegularSpell", Species.none},
            {"FireSpell", Species.none},
            {"WaterGoblin", Species.goblin},
            {"Kraken", Species.kraken},
            {"Knight", Species.knight},
            {"Wizzard", Species.wizzard},
            {"Ork", Species.ork},
            {"Dragon", Species.dragon},
            {"FireElf", Species.elf}
        };

        public override string ToString() { return $"Id: {Id}, Name: {Name}, Damage: {Damage}; "; }
        public string Id { get; set; } 
        public string Name { get; set; }
        public double Damage { get; set; }
        public Element Element => NameToElement[Name];
        public CardType CardType => NameToType[Name];
        public Species Species => NameToSpecies.GetValueOrDefault(Name);
    }
}
