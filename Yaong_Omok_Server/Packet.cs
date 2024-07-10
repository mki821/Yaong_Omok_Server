using System.Text.Json;

namespace Yaong_Omok_Server {
    public class Packet {
        public string Type { get; private set; }
        public object Data { get; private set; }

        public Packet(string type, object data) {
            Type = type;
            Data = data;
        }

        public string ToJson() => JsonSerializer.Serialize(this);
    }
}
