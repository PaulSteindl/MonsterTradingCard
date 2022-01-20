using HTTPServerCore.Routing.IProtectedRouteCommand;
using HTTPServerCore.Authentication.IIdentity;
using HTTPServerCore.Response.Response;
using MonsterTradingCard.Models.User;

namespace MonsterTradingCard.RouteCommands.ProtectedRouteCommand
{
    abstract class ProtectedRouteCommand : IProtectedRouteCommand
    {
        public IIdentity Identity { get; set; }

        public User User => (User)Identity;

        public abstract Response Execute();
    }
}
