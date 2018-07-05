using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSTimetable
{
    class JustTokenClass
    {
        public static string SQL_ConnectionString = "НУ ЧЕ В СОН ПАЦАНЫ";
        public static string SMS_Login = DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroLogin'");
        public static string SMS_APIKey =  DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroAPIKey'");
        public static string TG_APIKey = DatabaseLogicClass.GetSQL("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='TelegramAPIKey'");

    }
}
