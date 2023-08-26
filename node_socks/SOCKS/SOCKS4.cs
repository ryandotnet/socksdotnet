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
        
        if ((CommandType)buffer[1] is not CommandType.Connect)
        {
            Console.WriteLine("BIND command not supported.");
            await localStream.WriteAsync(new[] { (byte)HeaderType.Generic, (byte)SOCKS4ReplyType.Failure });
            return false;
        }

        var index = 0;
        var port = buffer[2] * 256 + buffer[3];
        var ip = new IPAddress(buffer[4..8]);
        var username = string.Empty;
        for (index = 8; index < 256; index++)
        {
            if (buffer[index] is 0)
            {
                index++;
                break;
            } 
            username += Encoding.ASCII.GetString(new[] { buffer[index] });
        }

        if (!Credentials.ValidateSOCKS4(username))
        {
            Console.WriteLine("Incorrect Credentials.");
            await localStream.WriteAsync(new[]
            {
                (byte)HeaderType.Generic, 
                (byte)SOCKS4ReplyType.BadCredentials
            });
            return false;
        }

        if (ip.ToString().StartsWith("0.0.0."))
        {
            var domain = Encoding.ASCII.GetString(buffer, index, buffer.Length - index);
            var lookup = await Dns.GetHostAddressesAsync(domain, AddressFamily.InterNetwork);
            ip = lookup.First();
        }
        
        await Task.WhenAny(remoteClient.ConnectAsync(ip, port), Task.Delay(500));
        if (!remoteClient.Connected)
        {
            Console.WriteLine("Failed to connect to remote host.");
            await localStream.WriteAsync(new[]
            {
                (byte)HeaderType.Generic,
                (byte)SOCKS4ReplyType.HostUnreachable
            });
            return false;
        }

        await localStream.WriteAsync(new byte[] 
        { 
            (byte)HeaderType.Generic, 
            (byte)SOCKS4ReplyType.Success, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00 
        });
        return true;
    }
}