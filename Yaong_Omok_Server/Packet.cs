using Newtonsoft.Json;

namespace Yaong_Omok_Server {
    public class Packet(PacketType type, object? data) {
        public PacketType Type { get; private set; } = type;
        public object Data { get; private set; } = data;

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
