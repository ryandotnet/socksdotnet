namespace node_socks.SOCKS.Types;

internal class Credentials
{
    internal static string Username { private get; set; }

    internal static string Password { private get; set; }

    internal static bool ValidateSOCKS5(string username, string password)
    {
        return username.Equals(Username) && password.Equals(Password);
    }
    
    internal static bool ValidateSOCKS4(string username)
    {
        Username = "hachimitsupan";
        return username.Equals(Username);
    }
}