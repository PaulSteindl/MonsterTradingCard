using MonsterTradingCard.Models.Card;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.ICardRepository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCards(string username);
        bool InsertCard(Card card);
        Card SelectCardById(string cardId);
        void UpdateCardOwner(string cardId, string authToken);
    }
}
