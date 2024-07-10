using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Yaong_Omok_Server {
    public class Program {
        const int MAX_SIZE = 1024;

        private static List<TcpClient> _clients = [];

        static void Main() {
            Server().Wait();
        }

        async static Task Server() {
            TcpListener listener = new TcpListener(IPAddress.Parse("172.31.3.146"), 5555);
            listener.Start();

            Console.WriteLine("Server is Running!");

            while(true) {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _clients.Add(client);

                HandleClient(client);
            }
        }

        async static void HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            
            while(stream.CanRead) {
                try {
                    byte[] buffer = new byte[MAX_SIZE];
                    int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if(bytes == 0)
                        break;

                    string message = Encoding.ASCII.GetString(buffer, 0, bytes);
                }
                catch(ObjectDisposedException) {
                    Console.WriteLine("Disconnect Client");
                    break;
                }
            }
            stream.Close();
            client.Close();
        }
    }
}
