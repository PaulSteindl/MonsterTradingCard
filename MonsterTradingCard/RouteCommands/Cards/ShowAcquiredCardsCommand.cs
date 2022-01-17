using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Card;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Cards.ShowAcquiredCardsCommand
{
    class ShowAcquiredCardsCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ShowAcquiredCardsCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            string jsonString = string.Empty;

            foreach (Card card in messageManager.GetCards(User.Token))
                jsonString += JsonConvert.SerializeObject(card);

            response.Payload = jsonString;
            response.StatusCode = StatusCode.Ok;

            return response;
        }
    }
}