using NUnit.Framework;
using MonsterTradingCard.Enums;
using MSCM = MonsterTradingCard.CardRelated.MonsterCard;
using MSCS = MonsterTradingCard.CardRelated.SpellCard;
using MonsterTradingCard.Battle.CardEffect;


namespace UnitsTests
{
    public class Tests
    {
        [TestFixture]
        public class CardEffectTests
        {
            [Test]
            /// Goblin + Dragon
            [TestCase(30, Element.Normal, CardType.Goblin, 20, Element.Normal, CardType.Dragon, ExpectedResult = 20)]
            [TestCase(10, Element.Normal, CardType.Goblin, 20, Element.Normal, CardType.Dragon, ExpectedResult = 10)]
            [TestCase(20, Element.Normal, CardType.Goblin, 20, Element.Normal, CardType.Dragon, ExpectedResult = 20)]
            /// Wizzard + Ork
            [TestCase(30, Element.Normal, CardType.Ork, 20, Element.Normal, CardType.Wizzard, ExpectedResult = -1)]
            [TestCase(10, Element.Normal, CardType.Ork, 20, Element.Normal, CardType.Wizzard, ExpectedResult = -1)]
            [TestCase(20, Element.Normal, CardType.Ork, 20, Element.Normal, CardType.Wizzard, ExpectedResult = -1)]
            /// Elves + Dragon 
            [TestCase(30, Element.Normal, CardType.Dragon, 20, Element.Fire, CardType.Elves, ExpectedResult = 20)]
            [TestCase(10, Element.Normal, CardType.Dragon, 20, Element.Fire, CardType.Elves, ExpectedResult = 10)]
            [TestCase(20, Element.Normal, CardType.Dragon, 20, Element.Fire, CardType.Elves, ExpectedResult = 20)]
            [TestCase(30, Element.Normal, CardType.Dragon, 20, Element.Normal, CardType.Elves, ExpectedResult = 30)]
            [TestCase(10, Element.Normal, CardType.Dragon, 20, Element.Normal, CardType.Elves, ExpectedResult = 10)]
            [TestCase(20, Element.Normal, CardType.Dragon, 20, Element.Normal, CardType.Elves, ExpectedResult = 20)]
            public int TestCardEffectMonsterAndMonster(byte dmg1, Element element1, CardType cardType1, byte dmg2, Element element2, CardType cardType2)
            {
                var card1 = new MSCM.MonsterCard(dmg1, element1, cardType1);
                var card2 = new MSCM.MonsterCard(dmg2, element2, cardType2);
                var cardeffect = new CardEffect(card1, card2);
                cardeffect.SpecialEffect();

                return card1.Dmg;
            }

            /// Knight + Spell
            [TestCase(30, Element.Normal, CardType.Knight, 20, Element.Water, ExpectedResult = -1)]
            [TestCase(10, Element.Normal, CardType.Knight, 20, Element.Water, ExpectedResult = -1)]
            [TestCase(20, Element.Normal, CardType.Knight, 20, Element.Water, ExpectedResult = -1)]
            [TestCase(30, Element.Normal, CardType.Knight, 20, Element.Normal, ExpectedResult = 30)]
            [TestCase(10, Element.Normal, CardType.Knight, 20, Element.Normal, ExpectedResult = 10)]
            [TestCase(20, Element.Normal, CardType.Knight, 20, Element.Normal, ExpectedResult = 20)]
            public int TestCardEffectMonsterAndSpell(byte dmg1, Element element1, CardType cardType1, byte dmg2, Element element2)
            {
                var card1 = new MSCM.MonsterCard(dmg1, element1, cardType1);
                var card2 = new MSCS.SpellCard(dmg2, element2);
                var cardeffect = new CardEffect(card1, card2);
                cardeffect.SpecialEffect();

                return card1.Dmg;
            }

            /// Spell + Kraken
            [TestCase(20, Element.Water, 20, Element.Fire, CardType.Kraken, ExpectedResult = -1)]
            [TestCase(20, Element.Fire, 20, Element.Fire, CardType.Kraken, ExpectedResult = -1)]
            [TestCase(20, Element.Normal, 20, Element.Fire, CardType.Kraken, ExpectedResult = -1)]
            public int TestCardEffectSpellAndMonster(byte dmg1, Element element1, byte dmg2, Element element2, CardType cardType2)
            {
                var card1 = new MSCS.SpellCard(dmg1, element1);
                var card2 = new MSCM.MonsterCard(dmg2, element2, cardType2);
                var cardeffect = new CardEffect(card1, card2);
                cardeffect.SpecialEffect();

                return card1.Dmg;
            }

            [TestCase(20, Element.Normal, CardType.Goblin, 20, Element.Normal, CardType.Elves, ExpectedResult = 20)]
            [TestCase(20, Element.Normal, CardType.Ork, 20, Element.Normal, CardType.Elves, ExpectedResult = 20)]
            [TestCase(20, Element.Normal, CardType.Goblin, 20, Element.Normal, CardType.Elves, ExpectedResult = 20)]
            [TestCase(20, Element.Normal, CardType.Kraken, 20, Element.Normal, CardType.Elves, ExpectedResult = 20)]
            [TestCase(20, Element.Normal, CardType.Werwolf, 20, Element.Normal, CardType.Acher, ExpectedResult = 20)]
            public int TestCardEffectNoValidEffectsOrOperations(byte dmg1, Element element1, CardType cardType1, byte dmg2, Element element2, CardType cardType2)
            {
                var card1 = new MSCM.MonsterCard(dmg1, element1, cardType1);
                var card2 = new MSCM.MonsterCard(dmg2, element2, cardType2);
                var cardeffect = new CardEffect(card1, card2);
                cardeffect.SpecialEffect();

                return card1.Dmg;
            }
        }
    }
}