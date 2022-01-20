using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.UserStats;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Card;
using MonsterTradingCard.Models.Package;
using MonsterTradingCard.Models.Highscore;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.Models.Deck;
using System.Collections.Generic;

namespace MonsterTradingCard.IMessageManager
{
    public interface IMessageManager
    {
        /// <summary>
        /// Logt User mit Credentials ein, wirft Exception wenn User nicht exisitert
        /// </summary>
        User LoginUser(Credentials credentials);
        /// <summary>
        /// Registriert User mit Credentials, wirft Exception wenn User bereits existiert
        /// </summary>
        void RegisterUser(Credentials credentials);
        /// <summary>
        /// Fügt eine Card mit Card in DB ein, wirft Exception falls Karte bereits existiert 
        /// </summary>
        void AddCard(Card card);
        /// <summary>
        /// Schaut ob jede Karte in List<Card> bereits existiert, wirft Exception falls Karte bereits existiert
        /// </summary>
        void CardExistence(List<Card> cards);
        /// <summary>
        /// Erstellt ein neues Pack, nimmt eine Liste an Cards entgegen
        /// </summary>
        void CreatePackage(List<Card> cards);
        /// <summary>
        /// Holt erstes Pack aus der Datenbank, ist null wenn es kein Pack gibt
        /// </summary>
        Package SelectFirstPackage();
        /// <summary>
        /// Schaut ob der User, welchem der Token gehört, genug Coins besitzt um ein Package zu kaufen
        /// </summary>
        bool CheckCoins(string authToken);
        /// <summary>
        /// Kauft Package mithilfe von PackageId und Usertoken und weist User die entsprechenden Karten zu
        /// </summary>
        void AcquirePackage(Package package, string authToken);
        /// <summary>
        /// Holt alle Karten von User mit Usertoken, leere Liste wenn user keine Karte besitzt
        /// </summary>
        IEnumerable<Card> GetCards(string authToken);
        /// <summary>
        /// Holt Deck von User mit Usertoken und retuniert eine Liste an Karten, leere Liste wenn user keine Karte besitzt
        /// </summary>
        List<Card> GetDeckReturnCardList(string authToken);
        /// <summary>
        /// Überprüft ob User diese Karte besitzt, true => User besitzt Karte
        /// </summary>
        bool CheckCardAndUser(string cardId, string authToken);
        /// <summary>
        /// Schaut ob User ein Deck hat
        /// </summary>
        bool UserDeckExists(string authToken);
        /// <summary>
        /// Aktualisiert Deck von User
        /// </summary>
        void UpdateDeck(string authToken, List<string> cardIds);
        /// <summary>
        /// Erstellt Deck für User
        /// </summary>
        int CreateDeck(string authToken, List<string> cardIds);
        /// <summary>
        /// Holt UserData von DB
        /// </summary>
        UserData GetUserData(string username);
        /// <summary>
        /// Aktualisiert UserData
        /// </summary>
        void UpdateUserData(string username, UserData userData);
        /// <summary>
        /// Holt Statistik für User, null wenn es User nicht gibt
        /// </summary>
        UserStats GetUserStats(string authToken);
        /// <summary>
        /// Holt Top 50 Highscores, leere liste wenn es keine gibt
        /// </summary>
        IEnumerable<Highscore> GetScore();
        /// <summary>
        /// Holt alle offenen Tradingdeals, leere liste wenn es keine gibt
        /// </summary>
        IEnumerable<TradingDeal> GetTradingDeals();
        /// <summary>
        /// Überprüft ob eine Karte gerade gehandelt wird true => Karte wird gehandelt
        /// </summary>
        bool CheckCardForTrade(string cardId);
        /// <summary>
        /// Erstellt einen Tradingdeal
        /// </summary>
        bool CreateTradingdeal(TradingDeal tradingDeal, string authToken);
        /// <summary>
        /// Löscht einen Tradingdeal
        /// </summary>
        bool DeleteTradingdeal(string tradingDealId, string authToken);
        /// <summary>
        /// Schaut ob Trading Deal exisitert, wirft eine exception falls Deal nicht exisiter
        /// </summary>
        TradingDeal CheckTradingdealExistsReturnDeal(string tradingDealId);
        /// <summary>
        /// Holt karte mit ID und Usertoken, ist null wenn Karte nicht existiert
        /// </summary>
        Card GetCardByIdAndToken(string cardId, string authToken);
        /// <summary>
        /// Aktualisiert besitzer der Karten mit Usertoken
        /// </summary>
        void UpdateCardOwnerById(string cardId, string authToken);
        /// <summary>
        /// Holt Deck mit token, ist null wenn es kein Deck gibt
        /// </summary>
        Deck GetDeck(string authToken);
        /// <summary>
        /// Updatet Stats um Win+1, Elo+3 und Scoreboard um score+1 wenn score bereits für User existiert ansonsten wird ein score für diesen User erstellt
        /// </summary>
        void UpdateStatsScoreWinner(string authToken, string username);
        /// <summary>
        /// Updatet Stats um Lose+1, Elo-5
        /// </summary>
        void UpdateStatsLoser(string authToken);
        /// <summary>
        /// Updatet Stats um Draw+1
        /// </summary>
        void UpdateStatsDraw(string authToken);
    }
}