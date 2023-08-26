using System.Net.Sockets;
using System.Text;
using node_socks.SOCKS.Types;

namespace node_socks;

internal class SOCKS5
{
    internal static async Task<SOCKS5ReplyType> Handshake(TcpClient client, TcpClient remote, byte[] buffer)
    {
        // Method negotiation
        for (var i = 2; i < 6; i++)
        {
            if ((AuthType)buffer[i] is AuthType.UserPass)
            {
                return await Authenticate(client, remote);
            }
        }

        return SOCKS5ReplyType.AuthNotSupported;
    }

    internal static async Task<SOCKS5ReplyType> Authenticate(TcpClient client, TcpClient remote)
    {
        var clientStream = client.GetStream();
        var buffer = new byte[256];
        
        await clientStream.WriteAsync(new byte[] { (byte)HeaderType.SOCKS5, (byte)AuthType.UserPass });
        await clientStream.ReadAsync(buffer);
        
        var usernameLength = buffer[1];
        var passwordLength = buffer[2 + usernameLength];
        var username = Encoding.ASCII.GetString(buffer, 2, usernameLength);
        var password = Encoding.ASCII.GetString(buffer, usernameLength + 3, passwordLength);

        if (!Credentials.ValidateSOCKS5(username, password))
        {
            Console.WriteLine("Incorrect credentials.");
            return SOCKS5ReplyType.Failure;
        }

        return SOCKS5ReplyType.Success;
    }
}