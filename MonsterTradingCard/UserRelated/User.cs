using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Abstract.Card;
using MonsterTradingCard.CardRelated.CardStack;


namespace MonsterTradingCard.UserRelated.User
{
    class User
    {
        public string username;

        private string Email;
        private Guid Token;
        private CardStack Deck;
        private CardStack Stack;
        public int Coins { get; set; }
    }
}
