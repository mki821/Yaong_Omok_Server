using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace Yaong_Omok_Server {
    public class Program {
        const int MAX_SIZE = 1024;

        public static List<Client> matchWatingClients = [];

        private static List<Client> _clients = [];
        private static System.Timers.Timer? _timer;

        private static void Main() {
            Server().Wait();
        }

        private async static Task Server() {
            TcpListener listener = new(IPAddress.Parse("172.31.3.146"), 5555);
            listener.Start();

            Console.WriteLine("Server is Running!");

            SetMatchMaking();

            while(true) {
                TcpClient client = await listener.AcceptTcpClientAsync();

                Console.WriteLine("Connect Client!");

                HandleClient(client);
            }
        }

        private static void SetMatchMaking() {
            _timer = new System.Timers.Timer(1000);

            _timer.Elapsed += new ElapsedEventHandler(OnMatchMaking);

            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnMatchMaking(Object? source, ElapsedEventArgs e) {
            if(matchWatingClients.Count == 0) return;

            MatchMaking.Match(matchWatingClients);
        }

        private async static void HandleClient(TcpClient tcpClient) {
            NetworkStream stream = tcpClient.GetStream();
            StringBuilder plus = new();
            Client client = new(tcpClient);
            _clients.Add(client);

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
                catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
            Console.WriteLine("Disconnect Client!");
            _clients.Remove(client);
            stream.Close();
            tcpClient.Close();
        }
    }
}
