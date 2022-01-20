using HTTPServerCore.Request.RequestContext;
using HTTPServerCore.Request.MethodUtilities;
using System.Collections.Generic;

namespace HTTPServerCore.Routing.IRouteParser
{
    public interface IRouteParser
    {
        bool IsMatch(RequestContext request, HttpMethod method, string routePattern);
        Dictionary<string, string> ParseParameters(RequestContext request, string routePattern);
    }
}
