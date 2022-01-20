using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.Package;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;

namespace MonsterTradingCard.RouteCommands.Packages.AcquirePackageCommand
{
    class AcquirePackageCommand : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;

        public AcquirePackageCommand(IMSGMANAGER.IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            Package package = null;

            package = messageManager.SelectFirstPackage();

            if(package != null)
            {
                if(package.CardIds.Count == 5)
                {
                    if(messageManager.CheckCoins(User.Token))
                    {
                        messageManager.AcquirePackage(package, User.Token);
                        response.StatusCode = StatusCode.Ok;
                        response.Payload = "Package wurde gekauft";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.Conflict;
                        response.Payload = "Nicht genügend coins!";
                    }
                }
                else
                {
                    response.StatusCode = StatusCode.InternalServerError;
                    response.Payload = "Es kam zu einem Fehler beim Laden des Packages!";
                }
            }
            else
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = "Kein Pack verfügbar!";
            }

            return response;
        }
    }
}
