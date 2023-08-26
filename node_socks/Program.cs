using Microsoft.Extensions.Configuration;
using node_socks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length is 0)
        {
            Console.WriteLine("Please specify a valid config file.");
        }
        
        if (args[0].Contains(".json"))
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + args[0]);
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Configuration file does not exist.");
                return;
            }

            if (!Configuration.Load(filePath))
            {
                return;
            }
        }

        await Server.StartServer();
    }
}