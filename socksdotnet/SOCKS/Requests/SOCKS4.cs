using System.Net;
using System.Net.Sockets;
using System.Text;
using socksdotnet.SOCKS.Types;

namespace socksdotnet.SOCKS.Requests;

internal class SOCKS4
{
    internal static async Task<SOCKS4ReplyType> HandleRequest(TcpClient client, TcpClient remote, byte[] buffer)
    {
        if ((CommandType)buffer[1] is not CommandType.Connect)
        {
            Console.WriteLine("BIND command not supported.");
            return SOCKS4ReplyType.Failure;
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
            return SOCKS4ReplyType.BadCredentials;
        }

        if (ip.ToString().StartsWith("0.0.0."))
        {
            var domain = Encoding.ASCII.GetString(buffer, index, buffer.Length - index);
            var lookup = await Dns.GetHostAddressesAsync(domain, AddressFamily.InterNetwork);
            ip = lookup.First();
        }

        await Task.WhenAny(remote.ConnectAsync(ip, port), Task.Delay(500));
        if (!remote.Connected)
        {
            Console.WriteLine("Failed to connect to remote host.");
            return SOCKS4ReplyType.HostUnreachable;
        }
        
        Console.WriteLine("{0} <--> {1}:{2}", client.Client.RemoteEndPoint, ip, port);
        return SOCKS4ReplyType.Success;
    }
}