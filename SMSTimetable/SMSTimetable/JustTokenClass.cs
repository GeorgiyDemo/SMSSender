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
        
        public static readonly string SMS_Login = DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroLogin'");
        public static readonly string SMS_APIKey =  DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroAPIKey'");
        public static readonly string Email_Login = DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='EmailLogin'");
        public static readonly string Email_Password = DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='EmailPassword'");
        public static readonly string TG_APIKey = DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='TelegramAPIKey'");
        public static readonly string TGProxy_ServerIP = DatabaseLogicClass.MySQLGet("SELECT Server FROM Proxies WHERE id=1");
        public static readonly string TGProxy_ServerPort = DatabaseLogicClass.MySQLGet("SELECT Port FROM Proxies WHERE id=1");
        public static readonly string TGProxy_User = DatabaseLogicClass.MySQLGet("SELECT User FROM Proxies WHERE id=1");
        public static readonly string TGProxy_Pass = DatabaseLogicClass.MySQLGet("SELECT Pass FROM Proxies WHERE id=1");
        public static readonly string MyMasterPassword = DatabaseLogicClass.MySQLGet("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='MasterPassword'");
        public static readonly string SQLiteDBPatch = "../../../../SMSTimetable.db";
    }
}
