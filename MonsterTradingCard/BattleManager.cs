using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.Deck;
using MonsterTradingCard.Models.FightLog;
using MonsterTradingCard.Models.Round;
using DECKINVALID = MonsterTradingCard.DeckNot4CardsException;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using MonsterTradingCard.Models.Enums.CardType;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.Models.Enums.CardEffect;
using MonsterTradingCard.Models.Enums.Species;
using System.Collections.Generic;
using MonsterTradingCard.Models.RoundDetailPlayer;
using System;

namespace MonsterTradingCard
{
    class BattleManager
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private List<Card> player = new List<Card>();
        private List<Card> opponent = new List<Card>();
        private string playerUsername;
        private string opponentUsername;
        private FightLog log;

        private Dictionary<Element, Element> Element_Dictionary = new Dictionary<Element, Element>()
        {
            {Element.water, Element.fire },
            {Element.fire, Element.normal },
            {Element.normal, Element.water }
        };

        public BattleManager(IMSGMANAGER.IMessageManager messageManager, Deck player, string playerUsername, Deck opponent, string opponentUsername)
        {
            this.messageManager = messageManager;

            log = new FightLog
            {
                Decks = new List<Deck>(),
                Rounds = new List<Round>()
            };

            log.Decks.Add(player);
            log.Decks.Add(opponent);

            this.player = SetupCards(player);
            this.opponent = SetupCards(opponent);

            this.playerUsername = playerUsername;
            this.opponentUsername = opponentUsername;
        }

        public FightLog Startbattle()
        {
            int roundNr = 1;
            var random = new Random();
            var tmpElementDmg = new List<float>();
            var tmpCardEffect = new CardEffect[2];

            for (; roundNr <= 100; roundNr++)
            {
                var newRound = new Round();
                newRound.RoundDetailPlayers = new List<RoundDetailPlayer>();

                //set roundNR
                newRound.RoundNumber = roundNr;

                //set username and Card for Player
                var newRoundDetailPlayer = new RoundDetailPlayer();
                newRoundDetailPlayer.Username = playerUsername;
                newRoundDetailPlayer.Card = player[random.Next() % player.Count];

                //set username and Card for Oppnent
                var newRoundDetailOpponent = new RoundDetailPlayer();
                newRoundDetailOpponent.Username = opponentUsername;
                newRoundDetailOpponent.Card = opponent[random.Next() % opponent.Count];

                if(newRoundDetailPlayer.Card.CardType == CardType.spell && newRoundDetailOpponent.Card.CardType == CardType.spell)
                {
                    //set ElementMultiplier
                    tmpElementDmg = ElementMultCalc(newRoundDetailPlayer.Card.Element, newRoundDetailOpponent.Card.Element);
                    newRoundDetailPlayer.ElementMultiplier = tmpElementDmg[0];
                    newRoundDetailOpponent.ElementMultiplier = tmpElementDmg[1];

                    //set TotalDamage
                    newRoundDetailPlayer.TotalDamage = newRoundDetailPlayer.Card.Damage * newRoundDetailPlayer.ElementMultiplier;
                    newRoundDetailOpponent.TotalDamage = newRoundDetailOpponent.Card.Damage * newRoundDetailOpponent.ElementMultiplier;

                    //set CardEffect to none
                    newRoundDetailPlayer.CardEffect = CardEffect.none;
                    newRoundDetailOpponent.CardEffect = CardEffect.none;
                }
                else
                {
                    //set CardEffect
                    tmpCardEffect = CheckCardEffect(newRoundDetailPlayer.Card, newRoundDetailOpponent.Card);
                    newRoundDetailPlayer.CardEffect = tmpCardEffect[0];
                    newRoundDetailOpponent.CardEffect = tmpCardEffect[1];

                    //set TotalDamage
                    newRoundDetailPlayer.TotalDamage = newRoundDetailPlayer.Card.Damage;
                    newRoundDetailOpponent.TotalDamage = newRoundDetailOpponent.Card.Damage;
                }

                //set winner, loser or draw and switches Cards
                SetWinnerLoserDraw(newRoundDetailPlayer, newRoundDetailOpponent, newRound);

                //set CardsLeft
                newRoundDetailPlayer.CardsLeft = player.Count;
                newRoundDetailOpponent.CardsLeft = opponent.Count;

                //add RoundDetails
                newRound.RoundDetailPlayers.Add(newRoundDetailPlayer);
                newRound.RoundDetailPlayers.Add(newRoundDetailOpponent);

                //add Round
                log.Rounds.Add(newRound);
            }
            //set total amount of numbers
            log.RoundCount = (roundNr == 101 ? 100 : roundNr);

            //set final winner
            if (player.Count <= 0)
            {
                log.Winner = opponentUsername;
                log.Loser = playerUsername;
                log.Draw = true;
            }
            else if(opponent.Count <= 0)
            {
                log.Winner = playerUsername;
                log.Loser = opponentUsername;
                log.Draw = true;
            }
            else
            {
                log.Winner = "none";
                log.Loser = "none";
                log.Draw = true;
            }
            return log;
        }

        private void SetWinnerLoserDraw(RoundDetailPlayer player, RoundDetailPlayer opponent, Round round)
        {
            if(player.CardEffect != CardEffect.none)
            {
                round.RoundWinner = opponent.Username;
                round.RoundLoser = player.Username;
                round.RoundDraw = false;

                this.opponent.Add(player.Card);
                this.player.Remove(player.Card);
            }
            else if(opponent.CardEffect != CardEffect.none)
            {
                round.RoundWinner = player.Username;
                round.RoundLoser = opponent.Username;
                round.RoundDraw = false;

                this.player.Add(opponent.Card);
                this.opponent.Remove(opponent.Card);
            }
            else 
            {
                if(player.TotalDamage > opponent.TotalDamage)
                {
                    round.RoundWinner = player.Username;
                    round.RoundLoser = opponent.Username;
                    round.RoundDraw = false;

                    this.player.Add(opponent.Card);
                    this.opponent.Remove(opponent.Card);                  
                }
                else if(player.TotalDamage < opponent.TotalDamage)
                {
                    round.RoundWinner = opponent.Username;
                    round.RoundLoser = player.Username;
                    round.RoundDraw = false;

                    this.opponent.Add(player.Card);
                    this.player.Remove(player.Card);
                }
                else
                {
                    round.RoundWinner = "none";
                    round.RoundLoser = "none";
                    round.RoundDraw = true;
                }
            }
        }

        private List<float> ElementMultCalc(Element playerElement, Element opponentElement)
        {
            if (opponentElement == Element_Dictionary[playerElement])
                return new List<float> { 2f, 0.5f };
            else if (playerElement == Element_Dictionary[opponentElement])
                return new List<float> { 0.5f, 2f };
            else
                return new List<float> { 1, 1 };
        }

        private CardEffect[] CheckCardEffect(Card playerCard, Card opponentCard)
        {
            int j = 1;
            var cardArray = new Card[2];
            var cardEffects = new CardEffect[2];
            cardArray[0] = playerCard;
            cardArray[1] = opponentCard;

            for (int i = 0; i < cardArray.Length; i++)
            {
                switch (cardArray[i].Species)
                {
                    case Species.goblin:
                        if (cardArray[j].Species == Species.dragon)
                        {
                            cardEffects[i] = CardEffect.scared;
                            cardEffects[j] = CardEffect.none;
                            return cardEffects;
                        }
                        break;

                    case Species.ork:
                        if (cardArray[j].Species == Species.wizzard)
                        {
                            cardEffects[i] = CardEffect.controlled;
                            cardEffects[j] = CardEffect.none;
                            return cardEffects;
                        }
                        break;

                    case Species.knight:
                        if (cardArray[j].CardType == CardType.spell && cardArray[j].Element == Element.water)
                        {
                            cardEffects[i] = CardEffect.drowned;
                            cardEffects[j] = CardEffect.none;
                            return cardEffects;
                        }
                        break;

                    case Species.kraken:
                        if (cardArray[i].CardType == CardType.spell)
                        {
                            cardEffects[i] = CardEffect.immune;
                            cardEffects[j] = CardEffect.none;
                            return cardEffects;
                        }
                        break;

                    case Species.elf:
                        if (cardArray[i].Element == Element.fire && cardArray[j].Species == Species.dragon)
                        {
                            cardEffects[i] = CardEffect.evade;
                            cardEffects[j] = CardEffect.none;
                            return cardEffects;
                        }
                        break;
                }
                j--;
            }
            cardEffects[0] = CardEffect.none;
            cardEffects[1] = CardEffect.none;
            return cardEffects;
        }

        private List<Card> SetupCards(Deck deck)
        {
            var cards = new List<Card>();

            foreach (string cardId in deck.CardIds)
                cards.Add(messageManager.GetCardByIdAndToken(cardId, deck.Token));

            if (cards.Count != 4)
                throw new DECKINVALID.DeckNot4CardsException();

            return cards;
        }

    }
}
