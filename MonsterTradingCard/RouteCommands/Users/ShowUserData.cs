using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using System.Linq;

namespace MonsterTradingCard.RouteCommands.Users.ShowUserData
{
    class ShowUserData : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private string username { get; set; }

        public ShowUserData(IMSGMANAGER.IMessageManager messageManager, string username)
        {
            this.messageManager = messageManager;
            this.username = username;
        }

        public override Response Execute()
        {
            var response = new Response();
            string tmpUsername = User.Token.Split('-').FirstOrDefault();

            if (username == tmpUsername)
            {
                string jsonString = JsonConvert.SerializeObject(messageManager.GetUserData(username));

                if (jsonString == string.Empty)
                {
                    response.StatusCode = StatusCode.NoContent;
                }
                else
                {
                    response.Payload = jsonString;
                    response.StatusCode = StatusCode.Ok;
                }
            }
            else
            {
                response.Payload = "Nicht berechtigt!";
                response.StatusCode = StatusCode.Unauthorized;
            }

            return response;
        }
    }
}
