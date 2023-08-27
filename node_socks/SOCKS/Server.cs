using System.Net;
using System.Net.Sockets;

namespace node_socks;

internal class Server
{
    internal static string IP { private get; set; }
    internal static int Port { private get; set; }
    internal static async Task StartServer()
    {
        var listener = new TcpListener(IPAddress.Parse(IP), Port)
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
        
        await HandleClient(listener);
    }

    private static async Task HandleClient(TcpListener listener)
    {
        try
        {
            var client = new TcpClient{ ReceiveBufferSize = 32768, SendBufferSize = 32768, NoDelay = true };
            var remote = new TcpClient{ ReceiveBufferSize = 32768, SendBufferSize = 32768, NoDelay = true };

            client = await listener.AcceptTcpClientAsync();

            if (!await Client.ParseRequest(client, remote))
            {
                client.Dispose();
                remote.Dispose();
                return;
            }
            
            _ = Task.Run(async () => await ExchangeData(client, remote));
            _ = Task.Run(async () => await ExchangeData(remote, client));
        }
        finally
        {
            await HandleClient(listener);
        }
    }
    
    private static async Task ExchangeData(TcpClient client, TcpClient remote)
    {
        int bytes;
        var buffer = new byte[32768];

        await using var clientStream = client.GetStream();
        await using var remoteStream = remote.GetStream();
        do
        {
            bytes = await clientStream.ReadAsync(buffer);
            await remoteStream.WriteAsync(buffer.AsMemory(0, bytes));
        } while (bytes is not 0);
    }
}