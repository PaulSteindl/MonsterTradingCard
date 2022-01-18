using System.Collections.Generic;
using MonsterTradingCard.Models.Highscore;

namespace MonsterTradingCard.DAL.IHighscoreRepository
{
    public interface IHighscoreRepository
    {
        public IEnumerable<Highscore> SelectHighscoreTop50();
    }
}
