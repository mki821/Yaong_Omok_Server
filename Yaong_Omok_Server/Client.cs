using System.Net.Sockets;

namespace Yaong_Omok_Server {
    public class Client {
        public TcpClient client;
        public Room? belongRoom;

        public Client(TcpClient client) {
            this.client = client;
        }
    }

    public class UserInfo {
        public string nickname;
        public string password;
        public int point;
    }
}
