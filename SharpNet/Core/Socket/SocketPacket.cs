using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Core.Socket
{
    [Serializable]
    public class SocketPacket
    {
        public int reqID;               // 요청아디
        public string reqUser;          // 요청유저아디
        public string reqData;          // 요청자료-일반데타.       
        public string EndSocket;

        public SocketPacket()
        {
            EndSocket = "<!--ESOCKET-->";
        }
    }

    [Serializable]
    public class LoginInfo : SocketPacket
    {
        public string nickName;
        public string pswd;
        public string macAddress;
        public string ipAddress;
    }

    [Serializable]
    public class GameUserInfo : SocketPacket
    {
        public DateTime? limitDate;
        public int userLevel;
        public string phoneNumber;
        public int money;
        public byte[] desData;
        public int workplayer;
    }
}
