using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using System.Linq;
using System.Text;

namespace MonsterTradingCard.RouteCommands.Messages.ListMessagesCommand
{
    class ListMessagesCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ListMessagesCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var messages = messageManager.ListMessages(User);

            var response = new Response();

            if (messages.Any())
            {
                var payload = new StringBuilder();
                foreach (var message in messages)
                {
                    payload.Append(message.Id);
                    payload.Append(": ");
                    payload.Append(message.Content);
                    payload.Append("\n");
                }
                response.StatusCode = StatusCode.Ok;
                response.Payload = payload.ToString();
            }
            else
            {
                response.StatusCode = StatusCode.NoContent;
            }

            return response;
        }
    }
}
