using Newtonsoft.Json;
using HTTPServerCore.Response.Response;
using HTTPServerCore.Response.StatusCode;
using MonsterTradingCard.Models.UserData;
using IMSGMANAGER = MonsterTradingCard.IMessageManager;
using PROT_ROUTE_COM = MonsterTradingCard.RouteCommands.ProtectedRouteCommand;
using System.Linq;

namespace MonsterTradingCard.RouteCommands.Users.UpdateUserData
{
    class UpdateUserData : PROT_ROUTE_COM.ProtectedRouteCommand
    {
        private readonly IMSGMANAGER.IMessageManager messageManager;
        private string username { get; set; }
        private UserData userData { get; set; }
        public UpdateUserData(IMSGMANAGER.IMessageManager messageManager, string username, UserData userData)
        {
            this.messageManager = messageManager;
            this.username = username;
            this.userData = userData;
        }

        public override Response Execute()
        {
            var response = new Response();
            string tmpUsername = User.Token.Split('-').FirstOrDefault();

            if (username == tmpUsername)
            {
                if(messageManager.GetUserData(username) != null)
                {
                    messageManager.UpdateUserData(username, userData);
                    response.Payload = "User wurde geupdated!";
                    response.StatusCode = StatusCode.Ok;
                }
                else
                {
                    response.Payload = "User existiert nicht!";
                    response.StatusCode = StatusCode.NotFound;
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
