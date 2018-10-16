using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для SenderWindow.xaml
    /// </summary>
    public partial class SenderWindow : Window
    {
        bool TelegramEnabledForm;
        TelegramClass TG_obj;

        public SenderWindow(TelegramClass recieved, bool TelegramEnabled)
        {
            TelegramEnabledForm = TelegramEnabled;
            TG_obj = recieved;
            InitializeComponent();
        }

        private void TextSecondRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FinalSendButton_Click(object sender, RoutedEventArgs e)
        {
            if ((TextSecondRadioButton.IsChecked == true) && (NumbersSecondRadioButton.IsChecked == true))
            {
                if (MessageBox.Show("Вы действительно хотите отправить сообщение '" + TextToSend.Text + "' на номер " + NumbersToSend.Text + "?", "Отправка сообщения", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SMSSenderClass sms_obj = new SMSSenderClass();
                    string[] numbers = new string[] { NumbersToSend.Text };
                    var request = new Request { numbers = numbers, text = TextToSend.Text, channel = "DIRECT" };
                    sms_obj.sms_send(request);
                    MessageBox.Show("Успешная отправка сообщения!");
                }
            }
            else if ((TextFirstRadioButton.IsChecked == true) && (NumbersSecondRadioButton.IsChecked == true))
            {
                MessageBox.Show("ВОШЛИ");
                string OutText = ParseJSONLogicClass.GetTimetableJSON();
                //SMSSenderClass sms_obj = new SMSSenderClass();
                //string[] numbers = new string[] { NumbersToSend.Text };
                //var request = new Request { numbers = numbers, text = OutText, channel = "DIRECT" };
                //sms_obj.sms_send(request);
                MessageBox.Show("Успешная отправка сообщения!");

            }


            GetSMSBalance();
            TextToSend.Text = "";
            NumbersToSend.Text = "";

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            GetSMSBalance();
            DispatcherTimer SMSTimer = new DispatcherTimer();
            SMSTimer.Tick += new EventHandler(SMSTimer_Tick);
            SMSTimer.Interval = new TimeSpan(0, 0, 60);
            SMSTimer.Start();

            TelegramServerLabel.Content = (TelegramEnabledForm == true) ? "Сервер Telegram: включен": "Сервер Telegram: выключен";

        }

        private void SMSTimer_Tick(object sender, EventArgs e)
        {
            GetSMSBalance();
            CommandManager.InvalidateRequerySuggested();
        }

        private void GetSMSBalance()
        {
            SMSSenderClass sms_obj = new SMSSenderClass();
            dynamic BalanceJSON = JObject.Parse(sms_obj.balance());
            BalanceLabel.Content = "Текущий баланс: " + BalanceJSON.data.balance + "₽";
        }

        private void SMSStatusButton_Click(object sender, RoutedEventArgs e)
        {
            SMSStatusWindow window_obj = new SMSStatusWindow();
            window_obj.Show();
        }


        private void TelegramServerLabel_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
           
            if (DatabaseLogicClass.SQLiteGet("SELECT boolvalue FROM servicetable WHERE service='TelegramService'") == "1")
            {
                DatabaseLogicClass.SQLiteExecute("UPDATE servicetable SET boolvalue = 0 WHERE service='TelegramService'");
                TelegramServerLabel.Content = "Сервер Telegram: выключен";
                TG_obj.TelegramInit(2);


            }
            else
            {
                DatabaseLogicClass.SQLiteExecute("UPDATE servicetable SET boolvalue = 1 WHERE service='TelegramService'");
                TelegramServerLabel.Content = "Сервер Telegram: включен";

                if (TG_obj == null)
                {
                    TG_obj = new TelegramClass();
                    TG_obj.TelegramInit(1);
                }

                TG_obj.TelegramInit(3);

            }
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagmentWindow UserWindow_obj = new UserManagmentWindow();
            UserWindow_obj.Show();
        }


    }
}
