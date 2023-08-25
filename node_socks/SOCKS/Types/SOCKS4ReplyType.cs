namespace node_socks.SOCKS.Types;

internal enum SOCKS4ReplyType
{
    Success = 0x5A,
    Failure = 0x5B,
    HostUnreachable = 0x5C,
    BadCredentials = 0x5D
}