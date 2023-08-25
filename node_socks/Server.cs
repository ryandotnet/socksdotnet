using System.Net;
using System.Net.Sockets;

namespace node_socks;

internal class Server
{
    internal async Task StartServer()
    {
        var listener = new TcpListener(IPAddress.Any, 1709)
        {
            Server =
            {
                ReceiveBufferSize = 32768,
                SendBufferSize = 32768,
                NoDelay = true
            }
        };
        listener.Start();
        await AcceptClientAsync(listener);
    }

    private async Task AcceptClientAsync(TcpListener listener)
    {
        var localClient = new TcpClient() { ReceiveBufferSize = 32768, SendBufferSize = 32768, NoDelay = true };
        var remoteClient = new TcpClient() { ReceiveBufferSize = 32768, SendBufferSize = 32768, NoDelay = true };

        localClient = await listener.AcceptTcpClientAsync();
    }
}