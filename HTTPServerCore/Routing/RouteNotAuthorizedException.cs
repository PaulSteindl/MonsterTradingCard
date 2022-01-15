using System;

namespace HTTPServerCore.Routing.RouteNotAuthorizedException
{
    class RouteNotAuthorizedException : Exception
    {
        public RouteNotAuthorizedException()
        {
        }

        public RouteNotAuthorizedException(string message) : base(message)
        {
        }

        public RouteNotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
