using MonsterTradingCard.DAL.IUserRepository;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using USER_NOT_FOUND = MonsterTradingCard.UserNotFoundException;
using DUPUSER = MonsterTradingCard.DuplicateUserException;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.Models.Credentials;

namespace MonsterTradingCard.MessageManager
{
    public class MessageManager : IMSGMANAGER.IMessageManager
    {
        private readonly IUserRepository userRepository;

        public MessageManager(IUserRepository userRepository)
        {
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
    }
}
