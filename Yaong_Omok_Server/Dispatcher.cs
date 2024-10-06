using Newtonsoft.Json;

namespace Yaong_Omok_Server {
    public class Dispatcher : Singleton<Dispatcher> {
        public Dictionary<string, Room> rooms = [];

        public void Dispatch(Client client, string message) {
            Packet? packet = JsonConvert.DeserializeObject<Packet>(message);

            switch (packet.Type) {
                case PacketType.Register:
                    Register(client, packet.Data);
                    break;
                case PacketType.Login:
                    Login(client, packet.Data);
                    break;
                case PacketType.MatchStart:
                    MatchStart(client);
                    break;
                case PacketType.MatchCancel:
                    MatchCancel(client);
                    break;
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
                case PacketType.GetRoomInfo:
                    GetRoomInfo(client);
                    break;
                case PacketType.StartGame:
                    StartGame(client);
                    break;
                case PacketType.Move:
                    Move(client, packet.Data);
                    break;
            }
        }

        private static void Register(Client client, object data) {
            UserInfo? user = JsonConvert.DeserializeObject<UserInfo>(data.ToString());

            UserInfo? existUser = Database.SelectByNickname("Users", user.nickname);

            bool isExistUser = existUser == null;
            if(isExistUser) {
                Database.Insert("Users", $"'{user.nickname}', '{Database.SHA256HASH(user.password)}', {user.point}, {user.mmr}");
                client.userInfo = user;
            }

            Packet packet = new(PacketType.Register, isExistUser);

            Messenger.Send(client, packet.ToJson());
        }

        private static void Login(Client client, object data) {
            UserInfo? user = JsonConvert.DeserializeObject<UserInfo>(data.ToString());

            UserInfo? existUser = Database.SelectByNickname("Users", user.nickname);

            bool isCorrect = existUser != null && Database.SHA256HASH(user.password) == existUser.password;

            if(isCorrect) client.userInfo = user;

            Packet packet = new(PacketType.Login, isCorrect);

            Messenger.Send(client, packet.ToJson());
        }

        private static void MatchStart(Client client) {
            Program.matchWatingClients.Add(client);

            Console.WriteLine($"{client.userInfo.nickname} joined the Queueing.");

            Packet packet = new(PacketType.MatchStart, true);

            Messenger.Send(client, packet.ToJson());
        }
        
        private static void MatchCancel(Client client) {
            Program.matchWatingClients.Remove(client);

            Console.WriteLine($"{client.userInfo.nickname} left the Queueing.");

            Packet packet = new(PacketType.MatchCancel, true);

            Messenger.Send(client, packet.ToJson());
        }

        private void MakeRoom(Client client, object data) {
            RoomInfo? roomInfo = JsonConvert.DeserializeObject<RoomInfo>(data.ToString());

            Packet packet = new(PacketType.Error, ErrorType.MakeRoomFailure);

            if(!rooms.ContainsKey(roomInfo.ID)) {
                Room room = new(roomInfo, client);
                rooms.Add(room.ID, room);

                packet = new(PacketType.MakeRoomSuccess, roomInfo);

                Console.WriteLine($"Make Room Successful!");
                Console.WriteLine($"(ID: {room.ID}, Name: {room.Name}{((room.Password != "") ? $", Password: {room.Password}" : "")})");

                room.EnterClient(client);
            }

            Messenger.Send(client, packet.ToJson());
        }

        private void RefreshRoom(Client client) {
            List<RoomInfo> roomInfos = new List<RoomInfo>();

            foreach(Room room in rooms.Values) {
                roomInfos.Add(room.GetInfo());
            }

            Packet packet = new(PacketType.RefreshRoom, roomInfos);

            Messenger.Send(client, packet.ToJson());
        }

        private void EnterRoom(Client client, object data) {
            RoomInfo? roomInfo = JsonConvert.DeserializeObject<RoomInfo>(data.ToString());

            Packet? packet;

            if(rooms.TryGetValue(roomInfo.ID, out Room? room)) {
                if(room.Password == "" || room.Password == roomInfo.Password) {
                    packet = new(PacketType.EnterRoom, roomInfo);
                    room.EnterClient(client);
                }
                else {
                    packet = new(PacketType.EnterRoom, true);
                }
            }
            else packet = new(PacketType.Error, ErrorType.MissingRoom);

            Messenger.Send(client, packet.ToJson());
        }

        private void ExitRoom(Client client, object data) {
            string? roomID = JsonConvert.DeserializeObject<string>(data.ToString());

            Packet? packet;

            if(rooms.TryGetValue(roomID, out Room? room)) {
                room.ExitClient(client);

                if(room.IsEmpty)
                    rooms.Remove(room.ID);

                packet = new(PacketType.ExitRoom, true);
            }
            else packet = new(PacketType.Error, ErrorType.MissingRoom);

            Messenger.Send(client, packet.ToJson());
        }

        private static void GetRoomInfo(Client client) {
            (UserInfo, UserInfo) userInfos = (client.belongRoom.client1.userInfo, client.belongRoom.client2.userInfo);
            Packet packet = new(PacketType.GetRoomInfo, userInfos);

            Messenger.Send(client, packet.ToJson());
        }

        private void StartGame(Client client) {
            if(client.belongRoom == null) {
                Packet packet = new(PacketType.Error, ErrorType.MissingRoom);

                Messenger.Send(client, packet.ToJson());
            }
            else {
                client.belongRoom.GameStart();
            }
        }

        private static void Move(Client client, object data) {
            Move? move = JsonConvert.DeserializeObject<MovePacket>(data.ToString())?.GetMove();

            client.belongRoom.Board.Move(move);

            Packet packet;

            if(client.belongRoom.Board.IsEndGame) {
                client.belongRoom.GameEnd();

                packet = new(PacketType.EndGame, client.belongRoom.Board);
                Messenger.Send(client, packet.ToJson());
            }
            else {
                packet = new(PacketType.Move, client.belongRoom.Board.Board);
                Messenger.Send(client, packet.ToJson());

                packet = new(PacketType.MoveSuccess, client.belongRoom.Board.IsPrevMoveSuccess);
                Messenger.Send(client, packet.ToJson());
            }
        }
    }
}
