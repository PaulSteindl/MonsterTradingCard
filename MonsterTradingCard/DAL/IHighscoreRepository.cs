using System.Collections.Generic;
using MonsterTradingCard.Models.Highscore;

namespace MonsterTradingCard.DAL.IHighscoreRepository
{
    public interface IHighscoreRepository
    {
        IEnumerable<Highscore> SelectHighscoreTop50();
        Highscore SelectHighscoreByUsername(string username);
        int UpdateWinByOneByUsername(string username);
        int InsertWinOneByUsername(string username);

    }
}
