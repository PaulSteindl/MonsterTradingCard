using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;

namespace MonsterTradingCard.RouteCommands.Users.ShowUserStatsCommand
{
    class ShowUserStatsCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ShowUserStatsCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if((response.Payload = JsonConvert.SerializeObject(messageManager.GetUserStats(User.Token))) != null)
            {
                response.StatusCode = StatusCode.Ok;
            }
            else
            {
                response.Payload = "User nicht gefunden!";
                response.StatusCode = StatusCode.NotFound;
            }

            return response;
        }
    }
}