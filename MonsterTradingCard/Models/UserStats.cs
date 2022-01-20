namespace MonsterTradingCard.Models.UserStats
{
    public class UserStats
    {
        public string Username { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Draws { get; set; }
        public int Elo { get; set; }
    }
}
