using System.Net.Sockets;
using System.Text;

namespace Yaong_Omok_Server {
    public class Room {
        public TcpClient? client1;
        public TcpClient? client2;

        private GoBoard? _board;

        public bool CanStart() {
            return client1 != null && client2 != null;
        }

        public async void Broadcast(string message) {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client1.GetStream();
            await stream.WriteAsync(buffer);

            stream = client2.GetStream();
            await stream.WriteAsync(buffer);
        }

        public void GameStart() {
            _board = new GoBoard();

            Packet packet = new Packet("Start", 0);
            Broadcast(packet.ToJson());
        }
    }
}
