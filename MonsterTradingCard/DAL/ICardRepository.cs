using MonsterTradingCard.Models.Card;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.ICardRepository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCards(string username);
        void InsertCard(Card card);

    }
}
