using MonsterTradingCard.Models.User;

namespace MonsterTradingCard.DAL.IUserRepository
{
    public interface IUserRepository
    {
        User GetUserByCredentials(string username, string password);

        User GetUserByAuthToken(string authToken);

        bool InsertUser(User user);
    }
}
