using HTTPServerCore.Authentication.IIdentityProvider;
using HTTPServerCore.Authentication.IIdentity;
using HTTPServerCore.Request.RequestContext;
using MonsterTradingCard.Models.User;
using MonsterTradingCard.DAL.IUserRepository;

namespace MonsterTradingCard.MessageIdentityProvider
{
    class MessageIdentityProvider : IIdentityProvider
    {
        private readonly IUserRepository userRepository;

        public MessageIdentityProvider(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IIdentity GetIdentyForRequest(RequestContext request)
        {
            User currentUser = null;

            if (request.Header.TryGetValue("Authorization", out string authToken))
            {
                const string prefix = "Basic ";
                if (authToken.StartsWith(prefix))
                {
                    currentUser = userRepository.GetUserByAuthToken(authToken.Substring(prefix.Length));
                }
            }
            return currentUser;
        }
    }
}
