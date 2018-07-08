using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для SenderWindow.xaml
    /// </summary>
    public partial class SenderWindow : Window
    {
        public SenderWindow()
        {
            InitializeComponent();
        }

        private void TextSecondRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FinalSendButton_Click(object sender, RoutedEventArgs e)
        {
           SMSSenderClass sms_obj = new SMSSenderClass();
           string[] numbers = new string[] { NumbersToSend.Text };
           var request = new Request { numbers = numbers, text = TextToSend.Text, channel = "DIRECT" };
           sms_obj.sms_send(request);
           GetSMSBalance();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            GetSMSBalance();
            DispatcherTimer SMSTimer = new DispatcherTimer();
            SMSTimer.Tick += new EventHandler(SMSTimer_Tick);
            SMSTimer.Interval = new TimeSpan(0, 0, 60);
            SMSTimer.Start();
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
    }
}
