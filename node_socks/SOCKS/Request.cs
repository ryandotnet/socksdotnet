using System.Net.Sockets;
using node_socks.SOCKS;
using node_socks.SOCKS.Types;

namespace node_socks;

public class Request
{
    internal static async Task<bool> Negotiate(TcpClient localClient, TcpClient remoteClient)
    {
        var localStream = localClient.GetStream();
        var buffer = new byte[257];

        await localStream.ReadAsync(buffer);
        switch ((HeaderTypes)buffer[0])
        {
            case HeaderTypes.SOCKS4:
                if (buffer[4] is 0)
                {
                    Console.WriteLine("SOCKS4a Client Found");
                    return await SOCKS4a.Auth(localClient, remoteClient, buffer);
                }
                Console.WriteLine("Socks4 Client Found");
                return await SOCKS4.Auth(localClient, remoteClient, buffer);
            case HeaderTypes.SOCKS5:
                return false;
            default:
                return false;
        }
    }
}