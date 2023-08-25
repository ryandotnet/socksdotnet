using System.Net.Sockets;

namespace node_socks;

public class Request
{
    internal static async Task<bool> Negotiate(TcpClient localClient, TcpClient remoteClient)
    {
        var localStream = localClient.GetStream();
        var buffer = new byte[257];

        await localStream.ReadAsync(buffer);
        switch ((HeaderType)buffer[0])
        {
            case HeaderType.SOCKS4:
                Console.WriteLine("Socks4 Client Found");
                return await SOCKS4.Auth(localClient, remoteClient, buffer);
            case HeaderType.SOCKS5:
                return false;
            default:
                return false;
        }
    }
}