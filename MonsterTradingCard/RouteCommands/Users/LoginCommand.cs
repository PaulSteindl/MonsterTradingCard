using HTTPServerCore.Response.Response;
using HTTPServerCore.Routing.IRouteCommand;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Credentials;
using MonsterTradingCard.Models.User;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using USER_NOT_FOUND = MonsterTradingCard.UserNotFoundException;

namespace MonsterTradingCard.RouteCommands.Users.LoginCommand
{
    class LoginCommand : IRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public Credentials Credentials { get; private set; }

        public LoginCommand(IMSGMANAGER.IMessageManager messageManager, Credentials credentials)
        {
            Credentials = credentials;
            this.messageManager = messageManager;
        }

        public Response Execute()
        {
            User user;
            try
            {
                user = messageManager.LoginUser(Credentials);
            }
            catch (USER_NOT_FOUND.UserNotFoundException)
            {
                user = null;
            }

            var response = new Response();
            if (user == null)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = "Passwort oder Benutzer ungültig!";
            }
            else
            {
                response.StatusCode = StatusCode.Ok;
                response.Payload = user.Token;
            }

            return response;
        }
    }
}
