using IDENTITY = HTTPServerCore.Authentication.IIdentity;
using HTTPServerCore.Request.RequestContext;

namespace HTTPServerCore.Authentication.IIdentityProvider
{
    public interface IIdentityProvider
    {
        IDENTITY.IIdentity GetIdentyForRequest(RequestContext request);
    }
}
