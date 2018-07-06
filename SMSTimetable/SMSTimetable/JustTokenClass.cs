using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSTimetable
{
    class JustTokenClass
    {

        //не стреляй в меня, я просто тень
        public static readonly string SQL_ConnectionString = "настройки";
        
        public static readonly string SMS_Login = DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroLogin'");
        public static readonly string SMS_APIKey =  DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroAPIKey'");
        public static readonly string TG_APIKey = DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='TelegramAPIKey'");
        public static readonly string TGProxy_ServerIP = DatabaseLogicClass.GetSQL("SELECT Server FROM Proxies WHERE id=1");
        public static readonly string TGProxy_ServerPort = DatabaseLogicClass.GetSQL("SELECT Port FROM Proxies WHERE id=1");
        public static readonly string TGProxy_User = DatabaseLogicClass.GetSQL("SELECT User FROM Proxies WHERE id=1");
        public static readonly string TGProxy_Pass = DatabaseLogicClass.GetSQL("SELECT Pass FROM Proxies WHERE id=1");

    }
}
