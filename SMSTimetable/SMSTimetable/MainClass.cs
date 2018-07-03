using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace SMSTimetable
{
    class MainClass
    {
        static string VK_TOKEN = "";
        static string TG_TOKEN = "";
        static string SMS_TOKEN = "";

        public static async Task TestApiAsync()
        {
            var botClient = new Telegram.Bot.TelegramBotClient(VK_TOKEN);
            var me = await botClient.GetMeAsync();
            MessageBox.Show("Hello! My name is "+me.FirstName);
        }
    }
}
