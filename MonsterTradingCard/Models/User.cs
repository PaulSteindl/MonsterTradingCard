using HTTPServerCore.Authentication.IIdentity;

namespace MonsterTradingCard.Models.User
{
    public class User : IIdentity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public float Winrate { get; set; }
        public int Coins { get; set; }

        public string Token => $"{Username}-mtcgToken";
    }
}
