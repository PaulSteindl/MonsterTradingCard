﻿using HTTPServerCore.Authentication.IIdentity;

namespace MonsterTradingCard.Models.User
{
    public class User : IIdentity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Bio { get; set; }
        public string Img { get; set; }

        public string Token => $"{Username}-msgToken";
    }
}
