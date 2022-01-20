using IROUTER = HTTPServerCore.Routing.IRouter;
using IROUTECOM = HTTPServerCore.Routing.IRouteCommand;
using IPROT_ROUTE_COM = HTTPServerCore.Routing.IProtectedRouteCommand;
using IROUTEPARSE = HTTPServerCore.Routing.IRouteParser;
using ROUTEEXCEPT = HTTPServerCore.Routing.RouteNotAuthorizedException;
using HTTPServerCore.Request.MethodUtilities;
using HTTPServerCore.Request.RequestContext;
using HTTPServerCore.Authentication.IIdentityProvider;
using System;
using System.Collections.Generic;

namespace HTTPServerCore.Routing.Router
{
    public class Router : IROUTER.IRouter
    {
        private interface ICreator { }

        private class PublicCreator : ICreator
        {
            public CreatePublicRouteCommand Create { get; set; }
        }

        private class ProtectedCreator : ICreator
        {
            public CreateProtectedRouteCommand Create { get; set; }
        }

        private readonly Dictionary<Tuple<HttpMethod, string>, ICreator> routes;

        public delegate IROUTECOM.IRouteCommand CreatePublicRouteCommand(RequestContext request, Dictionary<string, string> parameters);
        public delegate IPROT_ROUTE_COM.IProtectedRouteCommand CreateProtectedRouteCommand(RequestContext request, Dictionary<string, string> parameters);

        private readonly IROUTEPARSE.IRouteParser routeParser;
        private readonly IIdentityProvider identityProvider;

        public Router(IROUTEPARSE.IRouteParser routeParser, IIdentityProvider identityProvider)
        {
            routes = new Dictionary<Tuple<HttpMethod, string>, ICreator>();
            this.routeParser = routeParser;
            this.identityProvider = identityProvider;
        }

        public void AddRoute(HttpMethod method, string routePattern, CreatePublicRouteCommand create)
        {
            var key = new Tuple<HttpMethod, string>(method, routePattern);
            var value = new PublicCreator() { Create = create };
            routes.Add(key, value);
        }

        public void AddProtectedRoute(HttpMethod method, string routePattern, CreateProtectedRouteCommand create)
        {
            var key = new Tuple<HttpMethod, string>(method, routePattern);
            var value = new ProtectedCreator() { Create = create };
            routes.Add(key, value);
        }

        public IROUTECOM.IRouteCommand Resolve(RequestContext request)
        {
            IROUTECOM.IRouteCommand command = null;

            foreach (var route in routes.Keys)
            {
                if (routeParser.IsMatch(request, route.Item1, route.Item2))
                {
                    var parameters = routeParser.ParseParameters(request, route.Item2);
                    var creator = routes[route];
                    command = creator switch
                    {
                        PublicCreator c => c.Create(request, parameters),
                        ProtectedCreator c => Protect(c.Create, request, parameters),
                        _ => throw new NotImplementedException()
                    };
                    break;
                }
            }

            return command;
        }

        private IPROT_ROUTE_COM.IProtectedRouteCommand Protect(CreateProtectedRouteCommand create, RequestContext request, Dictionary<string, string> parameters)
        {
            var identity = identityProvider.GetIdentyForRequest(request);

            var command = create(request, parameters);
            command.Identity = identity ?? throw new ROUTEEXCEPT.RouteNotAuthorizedException();

            return command;
        }
    }
}
