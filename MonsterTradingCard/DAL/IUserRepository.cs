using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.UserData;
using MonsterTradingCard.Models.UserStats;

namespace MonsterTradingCard.DAL.IUserRepository
{
    public interface IUserRepository
    {
        User GetUserByCredentials(string username, string password);
        User GetUserByAuthToken(string authToken);
        bool InsertUser(User user);
        int SelectCoinsByToken(string authToken);
        void UpdateCoinsByMinus5(string authToken);
        UserData SelectUserDataByUsername(string username);
        void UpdateUserDataByUsername(string username, UserData userData);
        UserStats SelectUserStatsByToken(string authToken);
    }
}
