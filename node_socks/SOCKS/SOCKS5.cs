using System.Net;
using System.Net.Sockets;

namespace node_socks;

/*
     Method Selection Message
     +----+----------+----------+
     |VER | NMETHODS | METHODS  |
     +----+----------+----------+
     | 1  |    1     | 1 to 255 |
     +----+----------+----------+

     Method Selection Reply
     +----+--------+
     |VER | METHOD |
     +----+--------+
     | 1  |   1    |
     +----+--------+
     o  X'00' NO AUTHENTICATION REQUIRED
     o  X'01' GSSAPI
     o  X'02' USERNAME/PASSWORD
     o  X'03' to X'7F' IANA ASSIGNED
     o  X'80' to X'FE' RESERVED FOR PRIVATE METHODS
     o  X'FF' NO ACCEPTABLE METHODS

     SOCKS Request
     +----+-----+-------+------+----------+----------+
     |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
     +----+-----+-------+------+----------+----------+
     | 1  |  1  | X'00' |  1   | Variable |    2     |
     +----+-----+-------+------+----------+----------+
     o  VER    protocol version: X'05'
     o  CMD
     o  CONNECT X'01'
     o  BIND X'02'
     o  UDP ASSOCIATE X'03'
     o  RSV    RESERVED
     o  ATYP   address type of following address
     o  IP V4 address: X'01'  octet length of 4
     o  DOMAINNAME: X'03'     first octet contains the number of octets of the following domain name
     o  IP V6 address: X'04'  octet length of 16
     o  DST.ADDR       desired destination address
     o  DST.PORT desired destination port in network octet order

     SOCKS Reply
     +----+-----+-------+------+----------+----------+
     |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
     +----+-----+-------+------+----------+----------+
     | 1  |  1  | X'00' |  1   | Variable |    2     |
     +----+-----+-------+------+----------+----------+
     o  VER    protocol version: X'05'
     o  REP    Reply field:
     o  X'00' succeeded
     o  X'01' general SOCKS server failure
     o  X'02' connection not allowed by ruleset
     o  X'03' Network unreachable
     o  X'04' Host unreachable
     o  X'05' Connection refused
     o  X'06' TTL expired
     o  X'07' Command not supported
     o  X'08' Address type not supported
     o  X'09' to X'FF' unassigned
     o  RSV    RESERVED
     o  ATYP   address type of following address 
     o  IP V4 address: X'01'
     o  DOMAINNAME: X'03'
     o  IP V6 address: X'04'
     o  BND.ADDR       server bound address
     o  BND.PORT       server bound port in network octet order
*/

internal enum Headers
{
    SOCKS4 = 0x04,
    SOCKS5 = 0x05
}

internal enum Methods
{
    NoAuth = 0x00, 
    GSSApi = 0x01,
    UserPass = 0x02,
    NoSupport = 0xFF
}

public class SOCKS5
{
    internal async Task<(bool, IPAddress, int)> Authenticate(TcpClient localClient)
    {
        var localStream = localClient.GetStream();
        byte[] sendBuffer;
        var receiveBuffer = new byte[257];

        await localStream.ReadAsync(receiveBuffer);
        switch ((Headers)receiveBuffer[0])
        {
            case Headers.SOCKS4:
                break;
            case Headers.SOCKS5:
                break;
            default:
                return (false, IPAddress.None, 0);
        }
    }
}