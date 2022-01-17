using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Card;
using DECKINVALID = MonsterTradingCard.DeckNot4CardsException;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using System;

namespace MonsterTradingCard.RouteCommands.Decks.ShowDeckCommand
{
    class ShowDeckCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ShowDeckCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            string jsonString = string.Empty;

            try
            {
                foreach (Card card in messageManager.GetDeck(User.Token))
                    jsonString += JsonConvert.SerializeObject(card);

                if (jsonString == string.Empty)
                {
                    response.Payload = "Leeres Deck!\n";
                    response.StatusCode = StatusCode.NoContent;
                }
                else
                {
                    response.Payload = jsonString;
                    response.StatusCode = StatusCode.Ok;
                }
            }
            catch(DECKINVALID.DeckNot4CardsException)
            {
                response.Payload = "Es ist ein Fehler aufgetreten\n";
                response.StatusCode = StatusCode.InternalServerError;
            }
                        
            return response;
        }
    }
}