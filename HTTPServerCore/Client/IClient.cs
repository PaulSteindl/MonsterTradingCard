using HTTPServerCore.Request.RequestContext;
using RESPONSE = HTTPServerCore.Response.Response;

namespace HTTPServerCore.Client.IClient
{
    public interface IClient
    {
        public RequestContext ReceiveRequest();
        public void SendResponse(RESPONSE.Response response);
    }
}
