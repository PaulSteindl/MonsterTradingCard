using MonsterTradingCard.Models.Deck;

namespace MonsterTradingCard.DAL.IDeckRepository
{
    public interface IDeckRepository
    {
        Deck GetDeckByToken(string authToken);
    }
}
