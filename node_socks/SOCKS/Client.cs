using System.Net.Sockets;
using node_socks.SOCKS.Requests;
using node_socks.SOCKS.Types;

namespace node_socks;

public class Client
{
    internal static async Task<bool> ParseRequest(TcpClient client, TcpClient remote)
    {
        var clientStream = client.GetStream();
        var buffer = new byte[256];
        
        await clientStream.ReadAsync(buffer);
        
        switch ((HeaderType)buffer[0])
        {
            case HeaderType.SOCKS4:
            {
                var reply = await SOCKS4.HandleRequest(client, remote, buffer);
                return await ReplySOCKS4(clientStream, reply);
                
            }
            case HeaderType.SOCKS5:
            {
                var reply = await SOCKS5.Handshake(client, remote, buffer);
                return await ReplySOCKS5(clientStream, reply);
            }
            default:
                return false;
        }
    }

    private static async Task<bool> ReplySOCKS4(Stream clientStream, SOCKS4ReplyType reply)
    {
        await clientStream.WriteAsync(new byte[]
        {
            (byte)HeaderType.Generic, 
            (byte)reply, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00 
        });
        
        return reply is SOCKS4ReplyType.Success;
    }
    
    private static async Task<bool> ReplySOCKS5(Stream clientStream, SOCKS5ReplyType reply)
    {
        var buffer = new byte[]
        {
            (byte)HeaderType.SOCKS5,
            (byte)reply,
            0x00,
            (byte)AddressType.IPv4,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        
        if (reply is SOCKS5ReplyType.AuthFailed) { buffer[0] = (byte)HeaderType.UserPass; }
        
        await clientStream.WriteAsync(buffer);
        return reply is SOCKS5ReplyType.Success;
    }
}