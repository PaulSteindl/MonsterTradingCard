using MonsterTradingCard.Models.Message;
using System.Collections.Generic;

namespace MonsterTradingCard.DAL.IMessageRepository
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetMessages(string username);
        Message GetMessageById(string username, int messageId);
        void InsertMessage(string username, Message message);
        void UpdateMessage(string username, Message message);
        void DeleteMessage(string username, int messageId);
    }
}
