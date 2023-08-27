namespace socksdotnet.SOCKS.Types;

internal enum HeaderType
{
    Generic = 0x00,
    UserPass = 0x01,
    SOCKS4 = 0x04,
    SOCKS5 = 0x05
}