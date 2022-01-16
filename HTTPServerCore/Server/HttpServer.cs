using ISERVER = HTTPServerCore.Server.IServer;
using HTTPServerCore.Listener.IListener;
using LISTENER = HTTPServerCore.Listener.HttpListener;
using RESPONSE = HTTPServerCore.Response.Response;
using STATUSCODE = HTTPServerCore.Response.StatusCode;
using HTTPServerCore.Routing.RouteNotAuthorizedException;
using HTTPServerCore.Routing.IRouter;
using HTTPServerCore.Client.IClient;
using System.Net;
using System;

namespace HTTPServerCore.Server.HttpServer
{
    public class HttpServer : ISERVER.IServer
    {
        private readonly IListener listener;
        private readonly IRouter router;
        private bool isListening;

        public HttpServer(IPAddress address, int port, IRouter router)
        {
            listener = new LISTENER.HttpListener(address, port);
            this.router = router;
        }

        public void Start()
        {
            Console.WriteLine("Warte auf Client...");

            listener.Start();
            isListening = true;

            while (isListening)
            {
                var client = listener.AcceptClient();
                HandleClient(client);
            }
        }

        public void Stop()
        {
            isListening = false;
            listener.Stop();
        }

        private void HandleClient(IClient client)
        {
            var request = client.ReceiveRequest();

            RESPONSE.Response response;
            try
            {
                var command = router.Resolve(request);
                if (command != null)
                {
                    response = command.Execute();
                }
                else
                {
                    response = new RESPONSE.Response()
                    {
                        StatusCode = STATUSCODE.StatusCode.BadRequest
                    };
                }
            }
            catch (RouteNotAuthorizedException)
            {
                response = new RESPONSE.Response()
                {
                    StatusCode = STATUSCODE.StatusCode.Unauthorized,
                    Payload = "Ungültiger User!"
                };
            }

            client.SendResponse(response);
        }
    }
}
