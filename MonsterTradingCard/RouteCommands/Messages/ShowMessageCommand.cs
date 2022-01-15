using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MSG_NOT_FOUND = MonsterTradingCard.MessageNotFoundException;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using MonsterTradingCard.Models.Message;

namespace MonsterTradingCard.RouteCommands.Messages.ShowMessageCommand
{
    class ShowMessageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public int MessageId { get; private set; }

        public ShowMessageCommand(IMSGMANAGER.IMessageManager messageManager, int messageId)
        {
            MessageId = messageId;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            Message message;
            try
            {
                message = messageManager.ShowMessage(User, MessageId);
            }
            catch (MSG_NOT_FOUND.MessageNotFoundException)
            {
                message = null;
            }

            var response = new Response();
            if (message == null)
            {
                response.StatusCode = StatusCode.NotFound;
            }
            else
            {
                response.Payload = message.Content;
                response.StatusCode = StatusCode.Ok;
            }

            return response;
        }
    }
}
