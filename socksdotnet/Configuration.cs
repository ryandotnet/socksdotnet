using Microsoft.Extensions.Configuration;
using socksdotnet.SOCKS;

namespace socksdotnet;

internal class Configuration
{
    internal static bool Load(string filePath)
    {
        try
        {
            var configurationItems = new List<string> { "IP", "Port", "Username", "Password" };
            var configuration = new ConfigurationBuilder().AddJsonFile(filePath, true).Build();
            
            foreach (var item in configurationItems.Where(item => configuration[item] is null))
            {
                Console.WriteLine("Invalid or missing {0} in configuration file.", item);
                return false;
            }

            Server.IP = configuration["ip"];
            Server.Port = Convert.ToInt32(configuration["port"]);
            Credentials.Username = configuration["username"];
            Credentials.Password = configuration["password"];

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            throw;
        }
    }
}