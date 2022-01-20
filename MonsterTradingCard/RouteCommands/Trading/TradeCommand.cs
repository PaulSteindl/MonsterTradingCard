using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.TradingDeal;
using MonsterTradingCard.Models.Card;
using INVALIDEAL = MonsterTradingCard.Exceptions.TradingdealDoesNotExistException;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Trading.TradeCommand
{
    class TradeCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private readonly string tradingDealId;
        private readonly string cardId;

        public TradeCommand(IMSGMANAGER.IMessageManager messageManager, string tradingDealId, string cardId)
        {
            this.messageManager = messageManager;
            this.tradingDealId = tradingDealId;
            this.cardId = cardId;
        }

        private bool CheckValidTrade(TradingDeal tradingDeal, Card cardForDeal)
        {
            return
                tradingDeal.MinimumDamage <= cardForDeal.Damage &&
                (tradingDeal.Element == cardForDeal.Element || tradingDeal.Element == null) &&
                (tradingDeal.Type == cardForDeal.CardType || tradingDeal.Type == null) &&
                (tradingDeal.Species == cardForDeal.Species || tradingDeal.Species == null);
        }

        public override Response Execute()
        {
            var response = new Response();
            TradingDeal tradingDeal;
            Card cardForDeal;
            try
            {
                if ((tradingDeal = messageManager.CheckTradingdealExistsReturnDeal(tradingDealId)) != null)
                {
                    if (User.Token != tradingDeal.Usertoken)
                    {
                        if ((cardForDeal = messageManager.GetCardByIdAndToken(cardId, User.Token)) != null)
                        {
                            if (CheckValidTrade(tradingDeal, cardForDeal))
                            {
                                messageManager.UpdateCardOwnerById(tradingDeal.CardToTrade, User.Token);
                                messageManager.UpdateCardOwnerById(cardForDeal.Id, tradingDeal.Usertoken);
                                if(messageManager.DeleteTradingdeal(tradingDeal.Id, tradingDeal.Usertoken))
                                {
                                    response.Payload = "Trade war erfolgreich!";
                                    response.StatusCode = StatusCode.Ok;
                                }
                                else
                                {
                                    messageManager.UpdateCardOwnerById(cardForDeal.Id, User.Token);
                                    messageManager.UpdateCardOwnerById(tradingDeal.CardToTrade, tradingDeal.Usertoken);
                                    response.Payload = "Deal wurde nicht gelöscht, trade wurde rückgängig gemacht!";
                                    response.StatusCode = StatusCode.InternalServerError;
                                }
                            }
                        }
                        else
                        {
                            response.Payload = "Ungültige Karte!";
                            response.StatusCode = StatusCode.Conflict;
                        }
                    }
                    else
                    {
                        response.Payload = "User kann nicht mit sich selbst handeln!";
                        response.StatusCode = StatusCode.Forbidden;
                    }
                }
                else
                {
                    response.Payload = "Deal existiert nicht!";
                    response.StatusCode = StatusCode.Conflict;
                }
            }
            catch(INVALIDEAL.TradingdealDoesNotExistException)
            {
                response.Payload = "Deal existiert nicht!";
                response.StatusCode = StatusCode.Conflict;
            }
            return response;
        }
    }
}
