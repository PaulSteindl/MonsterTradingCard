using MonsterTradingCard.Models.Card;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.IMessageRepository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCards(string username);
        void AccquireCard(string usernmae, string CardId);
        void TradeCard(string CardId);
    }
}
