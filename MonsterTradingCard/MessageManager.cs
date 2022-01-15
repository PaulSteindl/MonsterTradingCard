using MonsterTradingCard.DAL.IUserRepository;
using MonsterTradingCard.DAL.IMessageRepository;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using MSG_NOT_FOUND = MonsterTradingCard.MessageNotFoundException;
using USER_NOT_FOUND = MonsterTradingCard.UserNotFoundException;
using DUPUSER = MonsterTradingCard.DuplicateUserException;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.Message;
using System.Collections.Generic;

namespace MonsterTradingCard.MessageManager
{
    public class MessageManager : IMSGMANAGER.IMessageManager
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;

        public MessageManager(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
        }

        public User LoginUser(Credentials credentials)
        {
            var user = userRepository.GetUserByCredentials(credentials.Username, credentials.Password);
            return user ?? throw new USER_NOT_FOUND.UserNotFoundException();
        }

        public void RegisterUser(Credentials credentials)
        {
            var user = new User()
            {
                Username = credentials.Username,
                Password = credentials.Password
            };
            if (userRepository.InsertUser(user) == false)
            {
                throw new DUPUSER.DuplicateUserException();
            }
        }

        public Message AddMessage(User user, string content)
        {
            var message = new Message() { Content = content };
            messageRepository.InsertMessage(user.Username, message);

            return message;
        }

        public IEnumerable<Message> ListMessages(User user)
        {
            return messageRepository.GetMessages(user.Username);
        }

        public void RemoveMessage(User user, int messageId)
        {
            if (messageRepository.GetMessageById(user.Username, messageId) != null)
            {
                messageRepository.DeleteMessage(user.Username, messageId);
            }
            else
            {
                throw new MSG_NOT_FOUND.MessageNotFoundException();
            }
        }

        public Message ShowMessage(User user, int messageId)
        {
            Message message;
            return (message = messageRepository.GetMessageById(user.Username, messageId)) != null
                ? message
                : throw new MSG_NOT_FOUND.MessageNotFoundException();
        }

        public void UpdateMessage(User user, int messageId, string content)
        {
            Message message;
            if ((message = messageRepository.GetMessageById(user.Username, messageId)) != null)
            {
                message.Content = content;
                messageRepository.UpdateMessage(user.Username, message);
            }
            else
            {
                throw new MSG_NOT_FOUND.MessageNotFoundException();
            }
        }
    }
}
