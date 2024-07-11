using System.Net.Sockets;
using System.Text;

namespace Yaong_Omok_Server {
    public class RoomInfo(string id, string name, string password) {
        public string ID { get; private set; } = id;
        public string Name { get; private set; } = name;
        public string Password { get; private set; } = password;
    }

    public class Room(RoomInfo roomInfo, TcpClient client) {
        public string ID { get; private set; } = roomInfo.ID;
        public string Name { get; private set; } = roomInfo.Name;
        public string Password { get; private set; } = roomInfo.Password;

        public TcpClient? client1 = client;
        public TcpClient? client2;

        private GoBoard? _board;

        public bool IsFull {
            get => client1 != null && client2 != null;
        }

        public bool CanStart() {
            return client1 != null && client2 != null;
        }

        public void Broadcast(string message) {
            if(client1 != null) Messenger.Send(client1, message);
            if(client2 != null) Messenger.Send(client2, message);
        }

        public void EnterClient(TcpClient client) {
            lock(this) {
                if(client2 == null) client2 = client;
                else client1 = client;
            }
        }

        public void ExitClient(TcpClient client) {
            if(client1 == client) client1 = null;
            else if(client2 == client) client2 = null;
        }

        public void GameStart() {
            _board = new GoBoard();

            Packet packet = new(PacketType.StartRoom, 0);
            Broadcast(packet.ToJson());
        }
    }
}
