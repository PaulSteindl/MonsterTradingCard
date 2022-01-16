using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Card;
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
    }
}