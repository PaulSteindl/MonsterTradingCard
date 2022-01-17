using MonsterTradingCard.Models.Deck;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.IDeckRepository
{
    public interface IDeckRepository
    {
        Deck SelectDeckByToken(string authToken);
        void UpdateDeckByToken(string authToken, List<string> cardIds);
        int InsertDeck(string authToken, List<string> cardIds);
    }
}
