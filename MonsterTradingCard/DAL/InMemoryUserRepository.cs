using USERREPO = MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace MonsterTradingCard.DAL.InMemoryUserRepository
{
    public class InMemoryUserRepository : USERREPO.IUserRepository
    {
        private readonly List<User> users = new();

        public User GetUserByAuthToken(string authToken)
        {
            return users.SingleOrDefault(u => u.Token == authToken);
        }

        public User GetUserByCredentials(string username, string password)
        {
            return users.SingleOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool InsertUser(User user)
        {
            var inserted = false;

            if (GetUserByName(user.Username) == null)
            {
                users.Add(user);
                inserted = true;
            }

            return inserted;
        }

        private User GetUserByName(string username)
        {
            return users.SingleOrDefault(u => u.Username == username);
        }

        public int SelectCoinsByToken(string authToken) { return 0; }
        public void UpdateCoinsByMinus5(string authToken) { }
    }
}
