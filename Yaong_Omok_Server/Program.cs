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
            StringBuilder plus = new();
            
            while(stream.CanRead) {
                try {
                    byte[] buffer = new byte[MAX_SIZE];
                    StringBuilder output = new();

                    if(plus.ToString().Length > 0) {
                        output.Append(plus);
                        plus.Clear();
                    }

                    while(!output.ToString().Contains("||")) {
                        int nbytes = await stream.ReadAsync(buffer);

                        if(nbytes == 0) throw new Exception();

                        output.Append(Encoding.ASCII.GetString(buffer, 0, nbytes));
                    }

                    string[] datas = output.ToString().Replace("\0", "").Split("||");

                    for(int i = 0; i < datas.Length; ++i) {
                        string message = datas[i];

                        if(message.Length > 0) {
                            if(i == (datas.Length - 1)) plus.Append(message);
                            else Dispatcher.Instance.Dispatch(client, message);
                        }
                    }
                }
                catch { break; }
            }
            Console.WriteLine("Disconnect Client!");
            stream.Close();
            client.Close();
        }
    }
}
