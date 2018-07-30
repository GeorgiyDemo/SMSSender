using System;
using System.Linq;
using Telegram.Bot;
using System.Windows;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MihaZupan;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SMSTimetable
{
    public class TelegramClass
    {
        public static string SMS_number, SMS_message, SMS_User;
        private static HttpToSocks5Proxy TG_Proxy = new HttpToSocks5Proxy(JustTokenClass.TGProxy_ServerIP, Convert.ToInt32(JustTokenClass.TGProxy_ServerPort),JustTokenClass.TGProxy_User,JustTokenClass.TGProxy_Pass);
        private static TelegramBotClient Bot = new TelegramBotClient(JustTokenClass.TG_APIKey, TG_Proxy);
    
        public static bool IntChecker(string CheckInt)
        {
            try
            {
                Convert.ToInt32(CheckInt);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static async Task<Boolean> AdminCheckerAsync(string id)
        {
            string admins  = await DatabaseLogicClass.MySQLGetAsync("SELECT AdminTG FROM TGAdmins");
            string[] AdminArr = admins.Split(',');
            for (int i = 0; i < AdminArr.Length;i++)
                if (AdminArr[i] == id)
                    return true;
            return false;
        }

        public static void TelegramInit()
        {
            var me = Bot.GetMeAsync().Result;
            #if DEBUG
                MessageBox.Show("Telegram успешно запущен -> @"+me.Username);
            #endif

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            //Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text) return;

            bool AdminResult = await AdminCheckerAsync(message.Chat.Id.ToString());

            if (AdminResult == false) return;
                
                switch (message.Text.Split(' ').First())
                {

                    case "/sms":
                        try
                        {
                            SMS_number = (message.Text.Split(' '))[1];
                            SMS_message = (message.Text.Split('"'))[1];

                            ReplyKeyboardMarkup ReplyKeyboard = new[]
                            {
                            new[] { "Да", "Нет" },
                        };

                            await Bot.SendTextMessageAsync(
                                message.Chat.Id,
                                "Вы действительно хотите отправить сообщение \"" + SMS_message + "\" на номер " + SMS_number + "?",
                                replyMarkup: ReplyKeyboard);

                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(
                               message.Chat.Id,
                               "Синтаксис использования команды:\n/sms номер_телефона \"Сообщение\"");
                        }
                        break;

                    case "/balance":

                        SMSSenderClass sms_obj = new SMSSenderClass();
                        dynamic BalanceJSON = JObject.Parse(sms_obj.balance());
                        string out_str = "Текущий баланс: " + BalanceJSON.data.balance + "₽";

                        await Bot.SendTextMessageAsync(
                            message.Chat.Id,
                            out_str);
                        break;

                    //Доделать
                    case "/history":

                        await Bot.SendTextMessageAsync(
                            message.Chat.Id,
                            "Тут история сообщений,  но пока не доделал");
                        break;

                    case "Да":

                        if ((Convert.ToString(SMS_message) != null) || (Convert.ToString(SMS_number) != null))
                        {
                            SMSSenderClass TelegramSMS_obj = new SMSSenderClass();
                            string[] numbers = new string[] { SMS_number };
                            TelegramSMS_obj.sms_send(new Request { numbers = numbers, text = SMS_message, channel = "DIRECT" });
                            dynamic TelegramBalanceJSON = JObject.Parse(TelegramSMS_obj.balance());


                            await Bot.SendTextMessageAsync(
                             message.Chat.Id,
                             "Сообщение успешно отправлено, остаток на балансе " + TelegramBalanceJSON.data.balance + "₽",
                                replyMarkup: new ReplyKeyboardRemove());
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(
                             message.Chat.Id,
                             "Что-то пошло не так при отправке сообщения 😔\nПовторно воспользуйтесь командой /sms",
                                replyMarkup: new ReplyKeyboardRemove());
                        }

                        SMS_message = null;
                        SMS_User = SMS_message;

                        break;

                    case "/user":
                        try
                        {
                            SMS_User = (message.Text.Split(' '))[1];
                            if (IntChecker(SMS_User) == true)
                            {
                                ReplyKeyboardMarkup UserReplyKeyboard = new[]
                                {
                                new[] { "Добавление", "Удаление" },
                            };

                                await Bot.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Доступные действия с пользователем " + SMS_User + ":",
                                    replyMarkup: UserReplyKeyboard);
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(
                                    message.Chat.Id,
                                    "Некорректный ввод пользователя, попробуйте заново", replyMarkup: new ReplyKeyboardRemove());
                            }
                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(
                               message.Chat.Id,
                               "Синтаксис использования команды:\n/user id_пользователя", replyMarkup: new ReplyKeyboardRemove());
                        }

                        break;

                    case "Добавление":
                        if (await DatabaseLogicClass.MySQLExecuteAsync("INSERT INTO TGAdmins(AdminTG) VALUES('" + SMS_User + "')") == true)
                        {
                            await Bot.SendTextMessageAsync(
                                  message.Chat.Id,
                                  "Добавление пользователя " + SMS_User + " успешно", replyMarkup: new ReplyKeyboardRemove());
                        }

                        SMS_User = null;
                        break;

                    case "Удаление":
                        if (await DatabaseLogicClass.MySQLExecuteAsync("DELETE FROM TGAdmins WHERE AdminTG = '" + SMS_User + "'") == true)
                        {
                            await Bot.SendTextMessageAsync(
                                  message.Chat.Id,
                                  "Удаление пользователя " + SMS_User + " успешно", replyMarkup: new ReplyKeyboardRemove());
                        }
                        SMS_User = null;
                        break;





                    default:
                        const string usage = @"
Доступные команды:
/sms - отправка сообщения
/balance - текущий баланс
/history - последние отправленные сообщения
/user - управление пользователями";

                        await Bot.SendTextMessageAsync(
                            message.Chat.Id,
                            usage,
                            replyMarkup: new ReplyKeyboardRemove());
                        break;
                }
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}

