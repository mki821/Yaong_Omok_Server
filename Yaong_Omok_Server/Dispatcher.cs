using System.Net.Sockets;
using Newtonsoft.Json;

namespace Yaong_Omok_Server {
    public class Dispatcher : Singleton<Dispatcher> {
        public Dictionary<string, Room> rooms = [];

        public void Dispatch(TcpClient tcpClient, string message) {
            Packet? packet = JsonConvert.DeserializeObject<Packet>(message);

            Client client = new(tcpClient);

            switch (packet.Type) {
                case PacketType.MakeRoom:
                    MakeRoom(client, packet.Data);
                    break;
                case PacketType.RefreshRoom:
                    RefreshRoom(client);
                    break;
                case PacketType.EnterRoom:
                    EnterRoom(client, packet.Data);
                    break;
                case PacketType.ExitRoom:
                    ExitRoom(client, packet.Data);
                    break;
                case PacketType.StartRoom:
                    break;
                case PacketType.Move:
                    Move(client, packet.Data);
                    break;
            }
        }

        private void MakeRoom(Client client, object data) {
            RoomInfo? roomInfo = JsonConvert.DeserializeObject<RoomInfo>(data.ToString());

            Room room = new(roomInfo, client);

            Packet packet = new(PacketType.MakeRoomSuccess, roomInfo);

            Console.WriteLine($"Make Room Successful!");
            Console.WriteLine($"(ID: {room.ID}, Name: {room.Name}{((room.Password != "") ? $", Password: {room.Password}" : "")})");

            Messenger.Send(client, packet.ToJson());

            rooms.Add(room.ID ,room);
        }

        private void RefreshRoom(Client client) {
            Packet packet = new(PacketType.RefreshRoom, rooms);

            Messenger.Send(client, packet.ToJson());
        }

        private void EnterRoom(Client client, object data) {
            RoomInfo roomInfo = (RoomInfo)data;

            Packet? packet;

            if(rooms.TryGetValue(roomInfo.ID, out Room? room)) {
                if(room.Password == "" || room.Password == roomInfo.Password) {
                    packet = new(PacketType.EnterRoom, roomInfo);
                    room.EnterClient(client);
                }
                else {
                    packet = new(PacketType.EnterRoom, null);
                }
            }
            else packet = new(PacketType.Error, 1);

            Messenger.Send(client, packet.ToJson());
        }

        private void ExitRoom(Client client, object data) {
            string roomID = (string)data;

            Packet? packet;

            if(rooms.TryGetValue(roomID, out Room? room)) {
                room.ExitClient(client);

                packet = new(PacketType.ExitRoom, 1);
            }
            else packet = new(PacketType.Error, 1);

            Messenger.Send(client, packet.ToJson());
        }

        private void Move(Client client, object data) {
            Move? move = JsonConvert.DeserializeObject<Move>(data.ToString());

            client.belongRoom.Board.Move(move);
        }
    }
}
