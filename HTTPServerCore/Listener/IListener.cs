using HTTPServerCore.Client.IClient;

namespace HTTPServerCore.Listener.IListener
{
    public interface IListener
    {
        IClient AcceptClient();
        void Start();
        void Stop();
    }
}
