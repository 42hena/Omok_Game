using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok_Game
{
    enum en_PacketType : ushort
    {
        LoginRequest = 1,
        LoginResponse = 2,

        RoomListRequest = 101,
        RoomListResponse = 102,

        CreateRoomRequest = 201,
        CreateRoomResponse = 202,

        EnterRoomRequest = 301,
        EnterRoomResponse = 302,
        EnterRoomBroadCastAlarm = 303,
        EnterRoomGetRoomUserList = 304,
        EnterRoomGetRoomPlayerList = 305,

        LeaveRoomRequest = 401,
        LeaveRoomResponse = 402,
        LeaveRoomBroadCastAlarm = 403,

        ChatRequest = 501,
        ChatResponse = 502,

        PlayerRequest = 1001,
        SpectatorRequest = 1002,
        PlayerResponse = 1003,
        SpectatorResponse = 1004,

        ReadyRequest = 1101,
        ReadyResponse = 1102,

        CancelRequest = 1103,
        CancelResponse = 1104,

        GameStart = 1105,


        PlaceStoneRequest = 1108,
        PlaceStoneResponse = 1109,
        Record = 1110,
        GameOver = 1101,

        Echo = 60000
    }
}
