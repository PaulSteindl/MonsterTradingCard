using MESSAGEREPO = MonsterTradingCard.DAL.IMessageRepository;
using MonsterTradingCard.Models.Message;
using System.Collections.Generic;
using System.Linq;

namespace MonsterTradingCard.DAL.InMemoryMessageRepository
{
    public class InMemoryMessageRepository : MESSAGEREPO.IMessageRepository
    {
        private readonly Dictionary<string, List<Message>> userMessages = new();
        private int nextMessageId = 1;

        public void DeleteMessage(string username, int messageId)
        {
            var foundMessage = GetMessageById(username, messageId);
            if (foundMessage != null)
            {
                if (userMessages.TryGetValue(username, out List<Message> messages))
                {
                    messages.Remove(foundMessage);
                    if (messages.Count == 0)
                    {
                        userMessages.Remove(username);
                    }
                }
            }
        }

        public Message GetMessageById(string username, int messageId)
        {
            return GetMessages(username).SingleOrDefault(m => m.Id == messageId);
        }

        public IEnumerable<Message> GetMessages(string username)
        {
            if (userMessages.TryGetValue(username, out List<Message> messages))
            {
                return messages;
            }
            return Enumerable.Empty<Message>();
        }

        public void InsertMessage(string username, Message message)
        {
            if (GetMessageById(username, message.Id) == null)
            {
                if (!userMessages.TryGetValue(username, out List<Message> messages))
                {
                    messages = new();
                    userMessages.Add(username, messages);
                }
                message.Id = nextMessageId++;
                messages.Add(message);
            }
        }

        public void UpdateMessage(string username, Message message)
        {
            var foundMessage = GetMessageById(username, message.Id);
            foundMessage.Content = message.Content;
        }
    }
}
