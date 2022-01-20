using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Card;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using DUPCARD = MonsterTradingCard.Exceptions.DuplicateCardException;
using System.Collections.Generic;

namespace MonsterTradingCard.RouteCommands.Packages.CreatePackageCommand
{
    class CreatePackageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        public List<Card> Cards { get; private set; }

        public CreatePackageCommand(IMSGMANAGER.IMessageManager messageManager, List<Card> cards)
        {
            Cards = cards;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();

            if (User.Token == "admin-mtcgToken")
            {
                //erstellt Karten zuerst da ein foreinkey von Card auf Package existiert
                if (Cards != null && Cards.Count == 5)
                {
                    try
                    {
                        messageManager.CardExistence(Cards);
                        foreach (Card card in Cards)
                            messageManager.AddCard(card);
                        messageManager.CreatePackage(Cards);
                        response.StatusCode = StatusCode.Ok;
                        response.Payload = "Package wurde erfolgreich erstellt!";
                    }
                    catch (DUPCARD.DuplicateCardException e)
                    {
                        response.StatusCode = StatusCode.Conflict;
                        response.Payload = "Karten-Id existiert schon!" + e;
                    }
                }
                else if (Cards != null && Cards.Count > 0)
                {
                    response.StatusCode = StatusCode.Conflict;
                    response.Payload = "Es sind nicht genau 4 Karten!";
                }
                else
                {
                    response.StatusCode = StatusCode.NoContent;
                    response.Payload = "Leerer Request!";
                }
            }
            else
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = "Kein Admin!";
            }

            return response;
        }
    }
}
