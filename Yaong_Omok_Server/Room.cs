namespace Yaong_Omok_Server {
    public class RoomInfo(string id, string name, string password) {
        public string ID { get; private set; } = id;
        public string Name { get; private set; } = name;
        public string Password { get; private set; } = password;
    }

    public class Room(RoomInfo roomInfo, Client client) {
        public string ID { get; private set; } = roomInfo.ID;
        public string Name { get; private set; } = roomInfo.Name;
        public string Password { get; private set; } = roomInfo.Password;

        public Client? client1 = client;
        public Client? client2;

        private GoBoard _board;

        public GoBoard Board {
            get => _board;
            private set => _board = value;
        }

        public bool IsFull {
            get => client1 != null && client2 != null;
        }

        public bool CanStart() {
            return client1 != null && client2 != null;
        }

        public RoomInfo GetInfo() {
            return new RoomInfo(ID, Name, Password);
        }

        public void Broadcast(string message) {
            if(client1 != null) Messenger.Send(client1, message);
            if(client2 != null) Messenger.Send(client2, message);
        }

        public void EnterClient(Client client) {
            lock(this) {
                if(client2 == null) client2 = client;
                else client1 = client;

                client.belongRoom = this;
            }
        }

        public void ExitClient(Client client) {
            client.belongRoom = null;

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
