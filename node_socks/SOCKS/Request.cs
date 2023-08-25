using System.Net.Sockets;

namespace node_socks;

public class Request
{
    internal static async Task RequestAuth(TcpClient localClient)
    {
        var localStream = localClient.GetStream();
        var buffer = new byte[257];

        await localStream.ReadAsync(buffer);
        switch ((HeaderType)buffer[0])
        {
            case HeaderType.SOCKS4:
                break;
            case HeaderType.SOCKS5:
                break;
            default:
                break;
        }
    }
}