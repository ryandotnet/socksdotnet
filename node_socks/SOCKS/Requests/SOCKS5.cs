using System.Net;
using System.Net.Sockets;
using System.Text;
using node_socks.SOCKS.Types;

namespace node_socks.SOCKS.Requests;

internal class SOCKS5
{
    internal static async Task<SOCKS5ReplyType> Handshake(TcpClient client, TcpClient remote, byte[] buffer)
    {
        for (var i = 2; i < 6; i++)
        {
            if ((AuthType)buffer[i] is AuthType.UserPass)
            {
                return await Authenticate(client, remote);
            }
        }
        
        Console.WriteLine("No authentication methods supported.");
        return SOCKS5ReplyType.AuthNotSupported;
    }

    private static async Task<SOCKS5ReplyType> Authenticate(TcpClient client, TcpClient remote)
    {
        var clientStream = client.GetStream();
        var buffer = new byte[256];
        
        await clientStream.WriteAsync(new[] { (byte)HeaderType.SOCKS5, (byte)AuthType.UserPass });
        await clientStream.ReadAsync(buffer);
        
        if ((HeaderType)buffer[0] is not HeaderType.UserPass)
        {
            Console.WriteLine("Incorrect authentication method.");
            return SOCKS5ReplyType.AuthFailed;
        }
        
        var usernameLength = buffer[1];
        var passwordLength = buffer[2 + usernameLength];
        var username = Encoding.ASCII.GetString(buffer, 2, usernameLength);
        var password = Encoding.ASCII.GetString(buffer, usernameLength + 3, passwordLength);

        if (!Credentials.ValidateSOCKS5(username, password))
        {
            Console.WriteLine("Incorrect credentials.");
            return SOCKS5ReplyType.AuthFailed;
        }

        await clientStream.WriteAsync(new[] { (byte)HeaderType.UserPass, (byte)SOCKS5ReplyType.Success });
        return await HandleRequest(client, remote, clientStream);
    }

    private static async Task<SOCKS5ReplyType> HandleRequest(TcpClient client, TcpClient remote, Stream clientStream)
    {
        var buffer = new byte[256];
        var ip = IPAddress.None;
        var port = 0;
        await clientStream.ReadAsync(buffer);
        
        if ((HeaderType)buffer[0] is not HeaderType.SOCKS5)
        {
            Console.WriteLine("Incorrect SOCKS protocol.");
            return SOCKS5ReplyType.Failure;
        }
        
        if ((CommandType)buffer[1] is not CommandType.Connect)
        {
            Console.WriteLine((CommandType)buffer[1] + " is not supported.");
            return SOCKS5ReplyType.CommandNotSupported;
        }

        switch ((AddressType)buffer[3])
        {
            case AddressType.IPv4:
            {
                ip = new IPAddress(buffer[4..8]);
                port = buffer[8] * 256 + buffer[9];
                break;
            }
            case AddressType.DomainName:
            {
                var portIndex = 5 + buffer[4];
                var domain = Encoding.ASCII.GetString(buffer, 5, buffer[4]);
                
                var lookup = Dns.GetHostAddressesAsync(domain, AddressFamily.InterNetwork);
                await Task.WhenAny(lookup, Task.Delay(500));
                if (lookup.Status is not TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Failed to resolve hostname.");
                    return SOCKS5ReplyType.HostUnreachable;
                }

                ip = lookup.Result.First();
                port = buffer[portIndex] * 256 + buffer[portIndex + 1];
                break;
            }
            case AddressType.IPv6:
            {
                Console.WriteLine("IPv6 is not supported.");
                return SOCKS5ReplyType.AddressNotSupported;
            }
            default:
            {
                Console.WriteLine("Incorrect address type provided.");
                return SOCKS5ReplyType.AddressNotSupported;
            }
        }

        await Task.WhenAny(remote.ConnectAsync(ip, port), Task.Delay(500));
        if (!remote.Connected)
        {
            Console.WriteLine("Host unreachable.");
            return SOCKS5ReplyType.HostUnreachable;
        }

        Console.WriteLine("{0} <--> {1}:{2}", client.Client.RemoteEndPoint, ip, port);
        return SOCKS5ReplyType.Success;
    }
}