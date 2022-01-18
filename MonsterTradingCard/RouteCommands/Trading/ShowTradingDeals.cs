using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Trading.ShowTradingDeals
{
    class ShowTradingDeals : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ShowTradingDeals(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if ((response.Payload = JsonConvert.SerializeObject(messageManager.GetTradingDeals())) != null)
                response.StatusCode = StatusCode.Ok;
            else
                response.StatusCode = StatusCode.NotFound;

            return response;
        }
    }
}
