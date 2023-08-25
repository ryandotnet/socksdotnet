using System.Net;
using System.Net.Sockets;
using System.Text;
using node_socks.SOCKS.Types;

namespace node_socks;

public class SOCKS4
{
    internal static async Task<bool> Auth(TcpClient localClient, TcpClient remoteClient, byte[] buffer)
    {
        var localStream = localClient.GetStream();

        if ((CommandTypes)buffer[1] is not CommandTypes.Connect)
        {
            Console.WriteLine("Only CONNECT is supported"); // handle later
            return false;
        }

        var sendBuffer = new byte[8];
        sendBuffer[0] = 0x00;
        sendBuffer[1] = 0x5A;
        var port = buffer[2] * 256 + buffer[3];
        var ip = string.Empty;
        for (var i = 4; i < 8; i++)
        {
            ip += buffer[i] + (i != 7 ? "." : "");
        }
        
        var username = Encoding.ASCII.GetString(buffer, 8, 13);
        if (username is not Credentials.Username)
        {
            Console.WriteLine("Incorrect Username"); // handle later
        }

        var test = IPAddress.Parse(ip);
        Console.WriteLine(test + ":" + port);
        
        await Task.WhenAny(remoteClient.ConnectAsync(test, port), Task.Delay(500));
        if (!remoteClient.Connected)
        {
            Console.WriteLine("Failed to connect to remote host."); // handle later
            return false;
        }

        await localStream.WriteAsync(sendBuffer);
        return true;
    }
}