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
        User LoginUser(Credentials credentials);
        void RegisterUser(Credentials credentials);
        void AddCard(Card card);
        void CardExistence(List<Card> cards);
        void CreatePackage(List<Card> cards);
        Package SelectFirstPackage();
        bool CheckCoins(string authToken);
        void AcquirePackage(Package package, string authToken);
        IEnumerable<Card> GetCards(string authToken);
        List<Card> GetDeckReturnCardList(string authToken);
        bool CheckCardAndUser(string cardId, string authToken);
        bool UserDeckExists(string authToken);
        void UpdateDeck(string authToken, List<string> cardIds);
        int CreateDeck(string authToken, List<string> cardIds);
        UserData GetUserData(string username);
        void UpdateUserData(string username, UserData userData);
        UserStats GetUserStats(string authToken);
        IEnumerable<Highscore> GetScore();
        IEnumerable<TradingDeal> GetTradingDeals();
        bool CheckCardForTrade(string cardId);
        bool CreateTradingdeal(TradingDeal tradingDeal, string authToken);
        bool DeleteTradingdeal(string tradingDealId, string authToken);
        TradingDeal CheckTradingdealExistsReturnDeal(string tradingDealId);
        Card GetCardByIdAndToken(string cardId, string authToken);
        void UpdateCardOwnerById(string cardId, string authToken);
        Deck GetDeck(string authToken);
    }
}