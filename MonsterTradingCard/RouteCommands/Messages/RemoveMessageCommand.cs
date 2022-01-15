using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MSG_NOT_FOUND = MonsterTradingCard.MessageNotFoundException;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;

namespace MonsterTradingCard.RouteCommands.Messages.RemoveMessageCommand
{
    class RemoveMessageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public int MessageId { get; private set; }

        public RemoveMessageCommand(IMSGMANAGER.IMessageManager messageManager, int messageId)
        {
            MessageId = messageId;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            try
            {
                messageManager.RemoveMessage(User, MessageId);
                response.StatusCode = StatusCode.Ok;
            }
            catch (MSG_NOT_FOUND.MessageNotFoundException)
            {
                response.StatusCode = StatusCode.NotFound;
            }

            return response;
        }
    }
}
