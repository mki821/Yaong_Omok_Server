using System.Net.Sockets;
using System.Text;

namespace Yaong_Omok_Server {
    public class Messenger {
        public async static void Send(TcpClient client, string message) {
            byte[] buffer = Encoding.ASCII.GetBytes(message + "||");

            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(buffer);
        }
    }
}
