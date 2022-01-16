using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;

namespace MonsterTradingCard.IMessageManager
{
    public interface IMessageManager
    {
        User LoginUser(Credentials credentials);
        void RegisterUser(Credentials credentials);
    }
}