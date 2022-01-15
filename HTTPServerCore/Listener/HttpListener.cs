using ILISTENER = HTTPServerCore.Listener.IListener;
using HTTPServerCore.Client.IClient;
using HTTPServerCore.Client.HttpClient;
using System.Net;
using System.Net.Sockets;

namespace HTTPServerCore.Listener.HttpListener
{
    public class HttpListener : ILISTENER.IListener
    {
        private readonly TcpListener listener;

        public HttpListener(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
        }


        public IClient AcceptClient()
        {
            var client = listener.AcceptTcpClient();
            return new HttpClient(client);
        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
