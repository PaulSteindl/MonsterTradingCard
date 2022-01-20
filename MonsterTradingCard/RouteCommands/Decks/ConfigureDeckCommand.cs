using System.Collections.Generic;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Card;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using NOUSERCARD = MonsterTradingCard.Exceptions.NoCardUserCombinationException;
using CARDTRADE = MonsterTradingCard.Exceptions.CardIsAvailabelToTradeException;

namespace MonsterTradingCard.RouteCommands.Decks.ConfigureDeckCommand
{
    class ConfigureDeckCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private readonly List<string> cardIds;

        public ConfigureDeckCommand(IMSGMANAGER.IMessageManager messageManager, List<string> cardIds)
        {
            this.messageManager = messageManager;
            this.cardIds = cardIds;
        }

        public override Response Execute()
        {
            var response = new Response();
            int newDeckid;

            if (cardIds.Count == 4)
            {
                try
                {
                    foreach (string cardId in cardIds)
                        if (!messageManager.CheckCardAndUser(cardId, User.Token))
                            throw new NOUSERCARD.NoCardUserCombinationException();

                    foreach (string cardId in cardIds)
                        if (messageManager.CheckCardForTrade(cardId))
                            throw new CARDTRADE.CardIsAvailabelToTradeException(cardId);

                    if (messageManager.UserDeckExists(User.Token))
                    {
                        messageManager.UpdateDeck(User.Token, cardIds);
                        response.Payload = "Deck wurde geupdated!";
                        response.StatusCode = StatusCode.Ok;
                    }
                    else
                    {
                        newDeckid = messageManager.CreateDeck(User.Token, cardIds);
                        response.Payload = "Deck wurde erstellt, Id: " + newDeckid;
                        response.StatusCode = StatusCode.Ok;
                    }
                }
                catch (NOUSERCARD.NoCardUserCombinationException)
                {
                    response.Payload = "User besitzt Karte nicht!";
                    response.StatusCode = StatusCode.NotFound;
                }
                catch (CARDTRADE.CardIsAvailabelToTradeException e)
                {
                    response.Payload = $"Card {e} wird gerade gehandelt!";
                    response.StatusCode = StatusCode.Conflict;
                }
            }
            else
            {
                response.Payload = "Deck-input enthält nicht genau 4 Karten";
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}