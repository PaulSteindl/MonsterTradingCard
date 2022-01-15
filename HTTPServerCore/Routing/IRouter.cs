using IROUTECOM = HTTPServerCore.Routing.IRouteCommand;
using HTTPServerCore.Request.RequestContext;

namespace HTTPServerCore.Routing.IRouter
{
    public interface IRouter
    {
        IROUTECOM.IRouteCommand Resolve(RequestContext request);
    }
}
