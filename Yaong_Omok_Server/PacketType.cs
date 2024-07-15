namespace Yaong_Omok_Server {
    
    public enum PacketType : ushort {
        None = 0,
        Error,

        MakeRoom,
        MakeRoomSuccess,
        RefreshRoom,
        EnterRoom,
        ExitRoom,

        StartRoom,

        Move
    }
}
