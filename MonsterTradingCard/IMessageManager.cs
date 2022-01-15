using MonsterTradingCard.Models.Message;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;
using System.Collections.Generic;

namespace MonsterTradingCard.IMessageManager
{
    public interface IMessageManager
    {
        Message AddMessage(User user, string content);
        IEnumerable<Message> ListMessages(User user);
        User LoginUser(Credentials credentials);
        void RegisterUser(Credentials credentials);
        void RemoveMessage(User user, int messageId);
        Message ShowMessage(User user, int messageId);
        void UpdateMessage(User user, int messageId, string content);
    }
}