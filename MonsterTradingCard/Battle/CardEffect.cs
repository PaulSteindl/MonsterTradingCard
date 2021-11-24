using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.Enums;

namespace MonsterTradingCard.Battle
{
    enum CardEffectEnum { Scared, Controlled, Drowned, Immune, Evade };

    class CardEffect
    {
        private CardEffectEnum?[] EffectArray = new CardEffectEnum?[2];
        private Card[] CardArray = new Card[2];

        CardEffect(Card c1, Card c2)
        {
            CardArray[0] = c1;
            CardArray[1] = c2;
        }

        public void SpecialEffect()
        {
            EffectArray[0] = GetEffect(CardArray[0], CardArray[1]);
            EffectArray[1] = GetEffect(CardArray[1], CardArray[0]);

            if (EffectArray[0].HasValue && EffectArray[1].HasValue)
                throw new ArgumentException();
            else if (EffectArray[0].HasValue || EffectArray[1].HasValue)
                ApplieEffect();
        }

        private static CardEffectEnum? GetEffect(Card Card1, Card Card2)
        { 
            switch (Card1.CardType)
            {
                case CardType.Goblin:
                    if (Card2.CardType == CardType.Dragon)
                        return CardEffectEnum.Scared;
                    break;

                case CardType.Ork:
                    if (Card2.CardType == CardType.Wizzard)
                        return CardEffectEnum.Controlled;
                    break;

                case CardType.Knight:
                    if (Card2.CardType == CardType.Spell && Card2.Element == Element.Water)
                       return CardEffectEnum.Drowned;
                    break;

                case CardType.Kraken:
                    if (Card2.CardType == CardType.Spell)
                        return CardEffectEnum.Immune;
                    break;

                case CardType.Elves:
                    if (Card1.Element == Element.Fire && Card2.CardType == CardType.Dragon)
                        return CardEffectEnum.Evade;
                    break;

                default:
                    return null;
            }
            return null;
        }

        private void ApplieEffect()
        {
            int j = 2;
            for(int i = 0; i < 2; i++)
            {
                --j;

                if(EffectArray[i].HasValue)
                {
                    switch (EffectArray[i])
                    {
                        case CardEffectEnum.Controlled:
                            Console.WriteLine($"{CardArray[j].Name} kontrolliert {CardArray[i].Name}!\n");
                            CardArray[i].Dmg = -1;
                            break;

                        case CardEffectEnum.Drowned:
                            Console.WriteLine($"{CardArray[i].Name} ist ertrunken!\n");
                            CardArray[i].Dmg = -1;
                            break;

                        case CardEffectEnum.Immune:
                            Console.WriteLine($"{CardArray[j].Name} hat keinen Effekt auf {CardArray[i].Name}!\n");
                            CardArray[j].Dmg = -1;
                            break;

                        case CardEffectEnum.Evade:
                            if (CardArray[i].Dmg < CardArray[j].Dmg)
                            { 
                                Console.WriteLine($"{CardArray[i]} weicht dem Angriff von {CardArray[j].Name} aus!\n");
                                CardArray[j].Dmg = CardArray[i].Dmg;
                            }
                            break;

                        case CardEffectEnum.Scared:
                            if (CardArray[i].Dmg > CardArray[j].Dmg)
                            {
                                Console.WriteLine($"{CardArray[i]} hat zu viel Angst um anzugreifen!\n");
                                CardArray[i].Dmg = CardArray[j].Dmg;
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            throw new Exception("In ApplieEffect kamm es zu einem Fehler!");
        }
    }
}
