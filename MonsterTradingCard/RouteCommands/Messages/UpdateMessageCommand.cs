using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MSG_NOT_FOUND = MonsterTradingCard.MessageNotFoundException;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;

namespace MonsterTradingCard.RouteCommands.Messages.UpdateMessageCommand
{
    class UpdateMessageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        public string Content { get; private set; }
        public int MessageId { get; private set; }

        public UpdateMessageCommand(IMSGMANAGER.IMessageManager messageManager, int messageId, string content)
        {
            MessageId = messageId;
            Content = content;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            try
            {
                messageManager.UpdateMessage(User, MessageId, Content);
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
