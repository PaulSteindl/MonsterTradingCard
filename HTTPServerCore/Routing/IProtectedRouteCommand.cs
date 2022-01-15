using HTTPServerCore.Authentication.IIdentity;
using IROUTECOM = HTTPServerCore.Routing.IRouteCommand;

namespace HTTPServerCore.Routing.IProtectedRouteCommand
{
    public interface IProtectedRouteCommand : IROUTECOM.IRouteCommand
    {
        IIdentity Identity { get; set; }
    }
}
