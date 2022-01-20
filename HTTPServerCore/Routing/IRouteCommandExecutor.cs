using IROUTECOM = HTTPServerCore.Routing.IRouteCommand;
using RESPONSE = HTTPServerCore.Response.Response;

namespace HTTPServerCore.Routing.IRouteCommandExecutor
{
    public interface IRouteCommandExecutor
    {
        RESPONSE.Response ExecuteCommand(IROUTECOM.IRouteCommand command);
    }
}
