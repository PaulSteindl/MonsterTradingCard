using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;
using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.Deck;
using MonsterTradingCard.Models.Enums.CardEffect;
using MonsterTradingCard.Models.Enums.Species;
using MonsterTradingCard.Models.Enums.Element;
using MonsterTradingCard.BattleManager;
using MonsterTradingCard.MessageManager;
using MonsterTradingCard.DAL.Database;
using MonsterTradingCard.Exceptions.DuplicateCardException;
using MonsterTradingCard.Exceptions.TradingdealDoesNotExistException;
using MonsterTradingCard.Models.RoundDetailPlayer;
using MonsterTradingCard.Models.Round;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Package;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.UserStats;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.Models.Enums.CardType;
using System;

namespace UnitsTests
{
    public class Tests
    {
        //For Testobjects
        static Database db = new Database("Host=localhost;Port=5432;Username=postgres;Password=123;Database=swe1messagedb");
        static List<string> deck1CardIds = new List<string> { "deck1-card1", "deck1-card2", "deck1-card3", "deck1-card4" };
        static Deck deck1 = new Deck { Id = 1, Token = "username-1-mtcgToken", CardIds = deck1CardIds };
        static string username1 = "username-1";
        static List<string> deck2CardIds = new List<string> { "deck2-card1", "deck2-card2", "deck2-card3", "deck2-card4" };
        static Deck deck2 = new Deck { Id = 2, Token = "username-2-mtcgToken", CardIds = deck2CardIds };
        static string username2 = "username-2";

        //Testobjects
        static MessageManager messageManager = new MessageManager(db.UserRepository, db.CardRepository, db.PackageRepository, db.DeckRepository, db.HighscoreRepositroy, db.TradingdealRepository);
        static BattleManager battleManager = new BattleManager(messageManager, deck1, username1, deck2, username2);

        static public void DBreset()
        {
            db.UserRepository.TruncateAllAndRestartId("fYyhAF4Lof#J8zbxfYcCGUDO2IpYy?dkH&1");
        }

        public class BattleTests
        {
            [TestFixture]
            public class CardEffectTests
            {
                [Test]
                [TestCase("WaterGoblin", "Dragon", ExpectedResult = new CardEffect[] { CardEffect.scared, CardEffect.none })]
                [TestCase("Dragon", "WaterGoblin", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.scared })]
                [TestCase("Ork", "Wizzard", ExpectedResult = new CardEffect[] { CardEffect.controlled, CardEffect.none })]
                [TestCase("Wizzard", "Ork", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.controlled })]
                [TestCase("FireElf", "Dragon", ExpectedResult = new CardEffect[] { CardEffect.evade, CardEffect.none })]
                [TestCase("Dragon", "FireElf", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.evade })]
                public CardEffect[] TestCardEffectMonsterAndMonster(string card1Name, string card2Name)
                {
                    var card1 = new Card { Name = card1Name };
                    var card2 = new Card { Name = card2Name };
                    var cardeffect = battleManager.CheckCardEffect(card1, card2);

                    return cardeffect;
                }

                [Test]
                [TestCase("Kraken", "WaterSpell", ExpectedResult = new CardEffect[] { CardEffect.immune, CardEffect.none })]
                [TestCase("Kraken", "RegularSpell", ExpectedResult = new CardEffect[] { CardEffect.immune, CardEffect.none })]
                [TestCase("Kraken", "FireSpell", ExpectedResult = new CardEffect[] { CardEffect.immune, CardEffect.none })]
                [TestCase("WaterSpell", "Kraken", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.immune })]
                [TestCase("RegularSpell", "Kraken", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.immune })]
                [TestCase("FireSpell", "Kraken", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.immune })]
                [TestCase("Knight", "WaterSpell", ExpectedResult = new CardEffect[] { CardEffect.drowned, CardEffect.none })]
                [TestCase("WaterSpell", "Knight", ExpectedResult = new CardEffect[] { CardEffect.none, CardEffect.drowned })]
                public CardEffect[] TestCardEffectMonsterAndSpell(string card1Name, string card2Name)
                {
                    var card1 = new Card { Name = card1Name };
                    var card2 = new Card { Name = card2Name };
                    var cardeffect = battleManager.CheckCardEffect(card1, card2);

                    return cardeffect;
                }
            }

            [TestFixture]
            public class ElementMultClacTests
            {
                [Test]
                [TestCase(Element.fire, Element.normal, ExpectedResult = new float[2] { 2f, 0.5f })]
                [TestCase(Element.normal, Element.fire, ExpectedResult = new float[2] { 0.5f, 2f })]
                [TestCase(Element.normal, Element.water, ExpectedResult = new float[2] { 2f, 0.5f })]
                [TestCase(Element.water, Element.normal, ExpectedResult = new float[2] { 0.5f, 2f })]
                [TestCase(Element.water, Element.fire, ExpectedResult = new float[2] { 2f, 0.5f })]
                [TestCase(Element.fire, Element.water, ExpectedResult = new float[2] { 0.5f, 2f })]
                [TestCase(Element.fire, Element.fire, ExpectedResult = new float[2] { 1f, 1f })]
                [TestCase(Element.normal, Element.normal, ExpectedResult = new float[2] { 1f, 1f })]
                [TestCase(Element.water, Element.water, ExpectedResult = new float[2] { 1f, 1f })]
                public List<float> TestCardEffectMonsterAndMonster(Element card1Element, Element card2Element)
                {
                    var cardeffect = battleManager.ElementMultCalc(card1Element, card2Element);

                    return cardeffect;
                }
            }

            [TestFixture]
            public class TestSetWinnerLoserDrawRound
            {
                private RoundDetailPlayer playerRoundDetail;
                private RoundDetailPlayer opponentRoundDetail;
                private Round round;
                private Round compareRound;

                [Test]
                public void SetPlayerWinnerWithDmgTest()
                {
                    //Test setup
                    playerRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 50,
                        Username = username1
                    };

                    opponentRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username2
                    };

                    round = new Round();

                    compareRound = new Round
                    {
                        RoundWinner = username1,
                        RoundLoser = username2,
                        RoundDraw = false
                    };

                    //Test
                    battleManager.SetWinnerLoserDraw(playerRoundDetail, opponentRoundDetail, round);
                    Assert.AreEqual(JsonConvert.SerializeObject(round), JsonConvert.SerializeObject(compareRound));
                }

                [Test]
                public void SetOpponentWinnerWithDmgTest()
                {
                    //Test setup
                    playerRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username1
                    };

                    opponentRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 50,
                        Username = username2
                    };

                    round = new Round();

                    compareRound = new Round
                    {
                        RoundWinner = username2,
                        RoundLoser = username1,
                        RoundDraw = false
                    };

                    //test
                    battleManager.SetWinnerLoserDraw(playerRoundDetail, opponentRoundDetail, round);
                    Assert.AreEqual(JsonConvert.SerializeObject(round), JsonConvert.SerializeObject(compareRound));
                }

                [Test]
                public void SetPlayerWinnerWithEffectTest()
                {
                    playerRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username1
                    };

                    opponentRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.drowned,
                        TotalDamage = 20,
                        Username = username2
                    };

                    round = new Round();

                    compareRound = new Round
                    {
                        RoundWinner = username1,
                        RoundLoser = username2,
                        RoundDraw = false
                    };

                    battleManager.SetWinnerLoserDraw(playerRoundDetail, opponentRoundDetail, round);
                    Assert.AreEqual(JsonConvert.SerializeObject(round), JsonConvert.SerializeObject(compareRound));
                }

                public void SetOpponentWinnerWithEffectTest()
                {
                    playerRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.scared,
                        TotalDamage = 20,
                        Username = username1
                    };

                    opponentRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username2
                    };

                    round = new Round();

                    compareRound = new Round
                    {
                        RoundWinner = username2,
                        RoundLoser = username1,
                        RoundDraw = false
                    };

                    battleManager.SetWinnerLoserDraw(playerRoundDetail, opponentRoundDetail, round);
                    Assert.AreEqual(JsonConvert.SerializeObject(round), JsonConvert.SerializeObject(compareRound));
                }

                public void SetDrawTest()
                {
                    playerRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username1
                    };

                    opponentRoundDetail = new RoundDetailPlayer
                    {
                        CardEffect = CardEffect.none,
                        TotalDamage = 20,
                        Username = username2
                    };

                    round = new Round();

                    compareRound = new Round
                    {
                        RoundWinner = "none",
                        RoundLoser = "none",
                        RoundDraw = true
                    };

                    battleManager.SetWinnerLoserDraw(playerRoundDetail, opponentRoundDetail, round);
                    Assert.AreEqual(JsonConvert.SerializeObject(round), JsonConvert.SerializeObject(compareRound));
                }
            }

            //SetupCards wird in MessageTests getestet!
        }

        public class MessageTests
        {
            [TestFixture]
            public class CardEffectTests
            {
                static private User testUser, testUser2;
                static private Card testCard0, testCard1, testCard2, testCard3, testCard4, testCard5;
                static private List<Card> testcardList;
                static private Package testpackage;
                static private UserData testuserData;
                static private UserStats teststats;
                static private TradingDeal testdeal;

                //DB bitte händisch vor Tests leeren, nicht das Daten verloren gehen!

                [SetUp]
                public void SetUp()
                {                    
                    testUser = new User
                    {
                        Username = username1,
                        Password = username2
                    };

                    testUser2 = new User
                    {
                        Username = username2,
                        Password = username1
                    };

                    testCard0 = new Card()
                    {
                        Id = deck2CardIds[0],
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testCard1 = new Card()
                    {
                        Id = deck1CardIds[0],
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testCard2 = new Card()
                    {
                        Id = deck1CardIds[1],
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testCard3 = new Card()
                    {
                        Id = deck1CardIds[2],
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testCard4 = new Card()
                    {
                        Id = deck1CardIds[3],
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testCard5 = new Card()
                    {
                        Id = "deck1-card5",
                        Name = "WaterSpell",
                        Damage = 20
                    };

                    testcardList = new List<Card>
                    {
                        testCard1, testCard2, testCard3, testCard4, testCard5
                    };

                    testpackage = new Package
                    {
                        CardIds = new List<string> { deck1CardIds[0], deck1CardIds[1], deck1CardIds[2], deck1CardIds[3], testCard5.Id },
                        Id = 1
                    };

                    testuserData = new UserData
                    {
                        Bio = "test-bio",
                        Image = "test-img",
                        Name = "test-name"
                    };

                    teststats = new UserStats
                    {
                        Draws = 0,
                        Elo = 100,
                        Loses = 0,
                        Username = testUser.Username,
                        Wins = 0
                    };

                    testdeal = new TradingDeal
                    {
                        Id = "test-tradingDeal",
                        CardToTrade = testCard1.Id,
                        Element = Element.water,
                        MinimumDamage = 20,
                        Species = Species.goblin,
                        Type = CardType.monster,
                        Usertoken = testUser.Token
                    };
                }

                [Test, Order(1)]
                public void TestRegisterAndLoginUser()
                {
                    DBreset();

                    var credentials = new Credentials
                    {
                        Username = username1,
                        Password = username2
                    };

                    messageManager.RegisterUser(credentials);
                    var user = messageManager.LoginUser(credentials);
                    Assert.AreEqual(JsonConvert.SerializeObject(testUser), JsonConvert.SerializeObject(user));

                    var credentials2 = new Credentials
                    {
                        Username = username2,
                        Password = username1
                    };

                    messageManager.RegisterUser(credentials2);
                    user = messageManager.LoginUser(credentials2);
                    Assert.AreEqual(JsonConvert.SerializeObject(testUser2), JsonConvert.SerializeObject(user));
                }

                [Test, Order(2)]
                public void TestAddCard()
                {
                    messageManager.AddCard(testCard0);
                    Assert.Catch<DuplicateCardException>(() => messageManager.AddCard(testCard0));
                }

                [Test, Order(3)]
                public void TestCardExistence()
                {
                    var compList = new List<Card>
                    {
                        testCard0
                    };

                    Assert.Catch<DuplicateCardException>(() => messageManager.CardExistence(compList));
                }

                [Test, Order(4)]
                public void TestCreatePackageAndSelectFirstPackage()
                {
                    foreach (Card card in testcardList)
                        messageManager.AddCard(card);

                    messageManager.CreatePackage(testcardList);
                    var comparepack = messageManager.SelectFirstPackage();

                    Assert.AreEqual(JsonConvert.SerializeObject(comparepack), JsonConvert.SerializeObject(testpackage));
                }

                [Test, Order(5)]
                public void TestCheckCoins()
                {
                    Assert.IsTrue(messageManager.CheckCoins(testUser.Token));
                }

                [Test, Order(6)]
                public void TestAcquirePackageAndGetCards()
                {
                    messageManager.AcquirePackage(testpackage, testUser.Token);
                    var cardList = messageManager.GetCards(testUser.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(cardList), JsonConvert.SerializeObject(testcardList));
                }

                [Test, Order(7)]
                public void TestCheckCardAndUser()
                {
                    Assert.IsTrue(messageManager.CheckCardAndUser(deck1CardIds[0], testUser.Token));
                }

                [Test, Order(8)]
                public void TestCreateDeck()
                {
                    Assert.IsTrue(messageManager.CreateDeck(testUser.Token, deck1CardIds) > 0);
                }

                [Test, Order(9)]
                public void TestUpdateDeckAndGetDeckRetrunCardList()
                {
                    var tmp = testcardList[4];
                    testcardList.RemoveAt(4);

                    messageManager.UpdateDeck(testUser.Token, deck1CardIds);
                    var cardList = messageManager.GetDeckReturnCardList(testUser.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(cardList), JsonConvert.SerializeObject(testcardList));

                    testcardList.Add(tmp);
                }

                [Test, Order(10)]
                public void TestUserDeckExists()
                {
                    Assert.IsTrue(messageManager.UserDeckExists(testUser.Token));
                }

                [Test, Order(11)]
                public void TestUpdateUserDataAndGetUserData()
                {
                    messageManager.UpdateUserData(testUser.Username, testuserData);
                    var compareUserData = messageManager.GetUserData(testUser.Username);
                    Assert.AreEqual(JsonConvert.SerializeObject(compareUserData), JsonConvert.SerializeObject(testuserData));
                }

                [Test, Order(12)]
                public void TestGetUserStats()
                {
                    var compareStats = messageManager.GetUserStats(testUser.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(compareStats), JsonConvert.SerializeObject(teststats));
                }

                [Test, Order(13)]
                public void TestCheckCardForTrade()
                {
                    Assert.IsTrue(!messageManager.CheckCardForTrade(testdeal.CardToTrade));
                }

                [Test, Order(14)]
                public void TestCreateTradingDeal()
                {
                    Assert.IsTrue(messageManager.CreateTradingdeal(testdeal, testUser.Token));
                    Assert.IsTrue(messageManager.CheckCardForTrade(testdeal.CardToTrade));
                }

                [Test, Order(15)]
                public void TestGetTradingDeals()
                {
                    testdeal.Usertoken = null;
                    var testtradingDeals = new List<TradingDeal>
                    {
                        testdeal
                    };
                                        
                    var tradingListcompare = messageManager.GetTradingDeals();

                    Assert.AreEqual(JsonConvert.SerializeObject(testtradingDeals), JsonConvert.SerializeObject(tradingListcompare));
                }

                [Test, Order(16)]
                public void TestCheckTradingdealExistsReturnDeal()
                {
                    var compdeal = messageManager.CheckTradingdealExistsReturnDeal(testdeal.Id);
                    Assert.AreEqual(JsonConvert.SerializeObject(compdeal), JsonConvert.SerializeObject(testdeal));
                    Assert.Catch<TradingdealDoesNotExistException>(() => messageManager.CheckTradingdealExistsReturnDeal(testCard0.Id));
                }

                [Test, Order(17)]
                public void TestDeleteTradingDeal()
                {
                    Assert.IsTrue(messageManager.DeleteTradingdeal(testdeal.Id, testUser.Token));
                    Assert.IsTrue(!messageManager.CheckCardForTrade(testdeal.CardToTrade));
                }

                [Test, Order(18)]
                public void TestGetCardByIdAndToken()
                {
                    var cardcomp = messageManager.GetCardByIdAndToken(testCard1.Id, testUser.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(testCard1), JsonConvert.SerializeObject(cardcomp));
                }

                [Test, Order(19)]
                public void TestUpdateCardOwnerById()
                {
                    messageManager.UpdateCardOwnerById(testCard1.Id, testUser2.Token);
                    var cardcomp = messageManager.GetCardByIdAndToken(testCard1.Id, testUser2.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(testCard1), JsonConvert.SerializeObject(cardcomp));
                    messageManager.UpdateCardOwnerById(testCard1.Id, testUser.Token);
                }

                [Test, Order(20)]
                public void TestGetDeck()
                {
                    var deckcomp = messageManager.GetDeck(testUser.Token);
                    Assert.AreEqual(JsonConvert.SerializeObject(deck1), JsonConvert.SerializeObject(deckcomp));
                }
            }

        }
    }
}