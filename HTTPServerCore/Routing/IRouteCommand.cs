using RESPONSE = HTTPServerCore.Response.Response;

namespace HTTPServerCore.Routing.IRouteCommand
{
    public interface IRouteCommand
    {
        RESPONSE.Response Execute();
    }
}
