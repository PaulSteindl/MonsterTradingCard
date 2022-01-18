using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.TradingDeal;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Trading.CreateTradingDeal
{
    class CreateTradingDeal : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private readonly TradingDeal tradingDeal;

        public CreateTradingDeal(IMSGMANAGER.IMessageManager messageManager, TradingDeal tradingDeal)
        {
            this.messageManager = messageManager;
            this.tradingDeal = tradingDeal;
        }

        public override Response Execute()
        {
            var response = new Response();

            if(messageManager.CheckCardAndUser(tradingDeal.CardToTrade, User.Token))
            {
                if(!messageManager.CheckCardForTrade(tradingDeal.CardToTrade))
                {
                    if(messageManager.CreateTradingdeal(tradingDeal, User.Token))
                    {
                        response.Payload = "Deal wurde erstellt!";
                        response.StatusCode = StatusCode.Ok;
                    }
                    else
                    {
                        response.Payload = "Es kam zu einem Fehler beim erstellen des Deals!";
                        response.StatusCode = StatusCode.InternalServerError;
                    }
                }
                else
                {
                    response.Payload = "Karte wird schon gehandelt!";
                    response.StatusCode = StatusCode.Conflict;
                }
            }
            else
            {
                response.Payload = "User besitzt Karte nicht!";
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}
