namespace node_socks.SOCKS.Types;

internal enum SOCKS5ReplyType
{
    Success = 0x00,
    Failure = 0x01,
    NotAllowed = 0x02,
    Unreachable = 0x03,
    HostUnreachable = 0x04,
    Refused = 0x05,
    Expired = 0x06,
    CommandNotSupported = 0x07,
    AddressNotSupported = 0x08,
    BadCredentials = 0x09,
    BadAuthType = 0x1A,
    AuthNotSupported = 0xFF,
}