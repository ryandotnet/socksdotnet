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
    NotSupported = 0x07,
    AddressNotSupported = 0x08,
    AuthNotSupported = 0xFF
}