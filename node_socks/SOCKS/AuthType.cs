namespace node_socks;

internal enum AuthType
{
    None = 0x00,
    GSSApi = 0x01,
    UserPass = 0x02,
    IANA = 0x04,
    Unsupported = 0xFF
}