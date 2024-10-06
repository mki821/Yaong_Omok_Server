namespace Yaong_Omok_Server {
    
    public enum PacketType : ushort {
        None = 0,
        Error,

        Register,
        Login,

        MatchStart,
        MatchCancel,
        MatchSuccess,

        MakeRoom,
        MakeRoomSuccess,
        RefreshRoom,
        EnterRoom,
        ExitRoom,

        GetRoomInfo,

        StartGame,
        Move,
        MoveSuccess,
        EndGame,
    }

    public enum ErrorType : ushort {
        None = 0,
        
        MissingRoom,
        MakeRoomFailure
    }
}
