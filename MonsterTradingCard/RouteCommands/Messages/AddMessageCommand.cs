using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;

namespace MonsterTradingCard.RouteCommands.Messages.AddMessageCommand
{
    class AddMessageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        
        public string Message { get; private set; }

        public AddMessageCommand(IMSGMANAGER.IMessageManager messageManager, string message)
        {
            Message = message;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var message = messageManager.AddMessage(User, Message);

            var response = new Response()
            {
                StatusCode = StatusCode.Created,
                Payload = $"{message.Id}"
            };

            return response;
        }
    }
}
