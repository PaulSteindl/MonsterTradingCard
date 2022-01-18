using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.TradingDeal;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Trading.DeleteTradingDealCommand
{
    class DeleteTradingDealCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private readonly string tradingDealId;

        public DeleteTradingDealCommand(IMSGMANAGER.IMessageManager messageManager, string tradingDealId)
        {
            this.messageManager = messageManager;
            this.tradingDealId = tradingDealId;
        }

        public override Response Execute()
        {
            var response = new Response();

            if (messageManager.DeleteTradingdeal(tradingDealId, User.Token))
            {
                    response.Payload = "Deal wurde gelöscht!";
                    response.StatusCode = StatusCode.Ok;
            }
            else
            {
                response.Payload = "User hat Deal nicht erstellt nicht oder Deal existiert nicht!";
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}
