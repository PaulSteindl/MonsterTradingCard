using HTTPServerCore.Response.Response;
using HTTPServerCore.Routing.IRouteCommand;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Credentials;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using DUPLICATEUSER = MonsterTradingCard.DuplicateUserException;

namespace MonsterTradingCard.RouteCommands.Users.RegisterCommand
{
    class RegisterCommand : IRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        public Credentials Credentials { get; private set; }

        public RegisterCommand(IMSGMANAGER.IMessageManager messageManager, Credentials credentials)
        {
            Credentials = credentials;
            this.messageManager = messageManager;
        }

        public Response Execute()
        {
            var response = new Response();
            try
            {
                messageManager.RegisterUser(Credentials);
                response.StatusCode = StatusCode.Created;
                response.Payload = Credentials.Username + " erfolgreich erstellt";
            }
            catch (DUPLICATEUSER.DuplicateUserException)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = Credentials.Username + " existiert bereits!";
            }

            return response;
        }
    }
}
