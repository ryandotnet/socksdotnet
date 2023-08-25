using System.Net;
using System.Net.Sockets;
using System.Text;

namespace node_socks;

public class SOCKS4
{
    internal static async Task<bool> Auth(TcpClient localClient, TcpClient remoteClient, byte[] buffer)
    {
        var localStream = localClient.GetStream();

        if ((CommandType)buffer[1] is not CommandType.Connect)
        {
            Console.WriteLine("Only CONNECT is supported"); // handle later
            return false;
        }

        var sendBuffer = new byte[8];
        sendBuffer[0] = 0x00;
        sendBuffer[1] = 0x5A;
        Array.Copy(buffer, 2, sendBuffer, 2, 6);
        var port = buffer[2] * 256 + buffer[3];
        var ip = string.Empty;
        for (var i = 4; i < 8; i++)
        {
            ip += buffer[i] + (i != 7 ? "." : "");
        }

        if (buffer[4] is 0)
        {
            Console.WriteLine("DNS Resolution Required");
        }

        var username = Encoding.ASCII.GetString(buffer, 8, 13);
        if (username is not Credentials.Username)
        {
            Console.WriteLine("Incorrect Username"); // handle later
        }

        await Task.WhenAny(remoteClient.ConnectAsync(IPAddress.Parse(ip), port), Task.Delay(500));
        if (!remoteClient.Connected)
        {
            Console.WriteLine("Failed to connect to remote host."); // handle later
            return false;
        }

        await localStream.WriteAsync(sendBuffer);
        return true;
    }
}