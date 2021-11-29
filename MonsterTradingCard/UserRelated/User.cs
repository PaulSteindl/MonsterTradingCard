using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.CardRelated.CardStack;


namespace MonsterTradingCard.UserRelated.User
{
    public class User
    {
        public string Username { get; }

        private string Email;
        private Guid Token;
        public CardStack Deck { get; set; }
        private CardStack Stack;
        public int Coins { get; set; }
    }
}
