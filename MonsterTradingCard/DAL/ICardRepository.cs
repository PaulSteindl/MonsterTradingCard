using MonsterTradingCard.Models.Card;
using System.Collections.Generic;

{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCards(string username);
    }
}
