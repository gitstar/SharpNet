namespace SharpNet.Core.Socket
{
    public enum SocketID
    {
        // 특수처리 0 ~ 98
        MyLogin,        // 특별한 권한 가지 유저 로그인 -- 개발자용
        GamePatch,      // 게임갱신
        Update,
        Pause,
        Resume,
        version,
        Ping,

        // 게임관련 90 ~ 500
        // 로그인관련
        Connected = 90,
        ConnectFull,
        Register,
        UserLogin,
        ItemCount,
        CachChange,
        Logout,
        GameExit,
        GameRestore,
        GameScore,
        GameLevel,

        // 게임처리
        CreatRoom,
        OutRoom,
        EnterRoom,
        JoinGame,
        OutGame,
        workRoom,
        wokerPlayer,
        NormalPlayer,
        BadPlayer,
        // game state
        Jackpot,
        Bonus,
        Chat,
        Notice

    }

    public enum LoginStatus
    {
        LoginSucess = 0,
        LoginFalid,
        IDInvalid,
        PswdInvalid,
        LoginAlready,
        Expired,
        IDBlock,
        Denided,
        MacAddress,
        IPAddress,
        AcceptCancel,
        AcceptWait,
        AcceptBreak,
        BetCorrect,
        Update,
        FullUser,
        NoServer
    }
}
