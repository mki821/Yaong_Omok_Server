using System.Net.Sockets;

namespace Yaong_Omok_Server {
    public class Client(TcpClient client) {
        public TcpClient client = client;
        public Room? belongRoom;
        public UserInfo? userInfo;
    }

    public class UserInfo {
        public string? nickname;
        public string? password;
        public int point;
    }
}
