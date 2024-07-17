namespace Yaong_Omok_Server {
    
    public enum PacketType : ushort {
        None = 0,
        Error,

        Register,
        Login,

        MakeRoom,
        MakeRoomSuccess,
        RefreshRoom,
        EnterRoom,
        ExitRoom,

        StartRoom,

        Move
    }

    public enum ErrorType : ushort {
        None = 0,
        
        MissingRoom,
        MakeRoomFailure
    }
}
