using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace node_socks;

internal class Server
{
    internal static async Task StartServer()
    {
        var listener = new TcpListener(IPAddress.Loopback, 1709)
        {
            Server =
            {
                ReceiveBufferSize = 32768,
                SendBufferSize = 32768,
                NoDelay = true
            }
        };
        listener.Start();
        Console.WriteLine("Listening on: " + listener.LocalEndpoint);
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync(); 
            {
                client.ReceiveBufferSize = 32768;
                client.SendBufferSize = 32768;
                client.NoDelay = true;
            }
            _ = Task.Run(async () => await HandleClient(client));
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        var remote = new TcpClient { ReceiveBufferSize = 32768, SendBufferSize = 32768, NoDelay = true };

        await Client.ParseRequest(client, remote);
    }
    
    private static async Task ExchangeDataAsync(TcpClient localClient, TcpClient remoteClient)
    {
        int bytes;
        var buffer = new byte[32768];

        await using var localStream = localClient.GetStream();
        await using var remoteStream = remoteClient.GetStream();
        do
        {
            bytes = await localStream.ReadAsync(buffer);
            await remoteStream.WriteAsync(buffer.AsMemory(0, bytes));
        } while (bytes is not 0);
    }
}