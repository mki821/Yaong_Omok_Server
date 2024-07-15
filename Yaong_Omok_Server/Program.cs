using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Yaong_Omok_Server {
    public class Program {
        const int MAX_SIZE = 1024;

        private static List<TcpClient> _clients = [];

        private static void Main() {
            Server().Wait();
        }

        private async static Task Server() {
            TcpListener listener = new(IPAddress.Parse("172.31.3.146"), 5555);
            listener.Start();

            Console.WriteLine("Server is Running!");

            while(true) {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _clients.Add(client);

                Console.WriteLine("Connect Client!");

                HandleClient(client);
            }
        }

        private async static void HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            
            while(stream.CanRead) {
                try {
                    byte[] buffer = new byte[MAX_SIZE];
                    int nbytes = await stream.ReadAsync(buffer);

                    if(nbytes == 0)
                        break;

                    string message = Encoding.ASCII.GetString(buffer, 0, nbytes);

                    //Console.WriteLine(message);

                    message = message.Replace("||", "");

                    Dispatcher.Instance.Dispatch(client, message);
                }
                catch(ObjectDisposedException) {
                    break;
                }
            }
            Console.WriteLine("Disconnect Client!");
            stream.Close();
            client.Close();
        }
    }
}
