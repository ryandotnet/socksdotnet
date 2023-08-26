using System.Net.Sockets;
using node_socks.SOCKS;
using node_socks.SOCKS.Types;

namespace node_socks;

public class Client
{
    internal static async Task ParseRequest(TcpClient localClient, TcpClient remoteClient)
    {
        var localStream = localClient.GetStream();
        var buffer = new byte[256];
        var result = SOCKS4ReplyType.Success;

        await localStream.ReadAsync(buffer);
        switch ((HeaderType)buffer[0])
        {
            case HeaderType.SOCKS4:
            {
                result = await SOCKS4.Auth(localClient, remoteClient, buffer);
                break;
            }
            case HeaderType.SOCKS5:
                break;
        };
        await localStream.WriteAsync(new[] { (byte)HeaderType.Generic, (byte)result });
    }

    internal static async Task HandleSOCKS4Request(TcpClient localClient, TcpClient remoteClient, byte[] buffer)
    {
        var result = await SOCKS4.Negotiate(localClient, remoteClient, buffer);
        await 
    }
}