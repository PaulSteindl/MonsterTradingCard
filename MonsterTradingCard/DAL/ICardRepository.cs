using MonsterTradingCard.Models.Card;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.ICardRepository
{
    public interface ICardRepository
    {
        IEnumerable<Card> SelectCardsByToken(string username);
        bool InsertCard(Card card);
        Card SelectCardById(string cardId);
        void UpdateCardOwner(string cardId, string authToken);
        Card SelectCardByIdAndToken(string cardId, string authToken);
    }
}
