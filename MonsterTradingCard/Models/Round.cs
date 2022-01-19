using System.Collections.Generic;
using DETAIL = MonsterTradingCard.Models.RoundDetailPlayer;

namespace MonsterTradingCard.Models.Round
{
    class Round
    {
        public int RoundNumber { get; set; }
        public string RoundWinner { get; set; }
        public string RoundLoser { get; set; }
        public bool RoundDraw { get; set; }
        public List<DETAIL.RoundDetailPlayer> RoundDetailPlayers { get; set; }
    }
}
