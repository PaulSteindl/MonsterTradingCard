using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Card;
using DECKINVALID = MonsterTradingCard.Exceptions.DeckNot4CardsException;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Decks.ShowDeckPlainCommand
{
    class ShowDeckPlainCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public ShowDeckPlainCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            string responseString = string.Empty;

            try
            {
                foreach (Card card in messageManager.GetDeckReturnCardList(User.Token))
                    responseString += card.ToString();

                if (responseString == string.Empty)
                {
                    response.StatusCode = StatusCode.NoContent;
                }
                else
                {
                    response.Payload = responseString;
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