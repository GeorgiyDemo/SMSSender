using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для SenderWindow.xaml
    /// </summary>
    public partial class SenderWindow : Window
    {
        string LocalLoginedName;
        bool TelegramEnabledForm;
        TelegramClass TG_obj;

        public SenderWindow(TelegramClass recieved, bool TelegramEnabled, string LoginedName)
        {
            TelegramEnabledForm = TelegramEnabled;
            TG_obj = recieved;
            LocalLoginedName = "Авторизация: "+CryptoClass.Base64Decode(LoginedName);
            InitializeComponent();
        }

        private async void FinalSendButton_Click(object sender, RoutedEventArgs e)
        {

            //Отправка произовольного текста произвольному получателю
            if ((TextSecondRadioButton.IsChecked == true) && (NumbersSecondRadioButton.IsChecked == true))
            {
                if ((NumbersToSend.Text != "") && (TextToSend.Text != ""))
                {
                    if (MessageBox.Show("Вы действительно хотите отправить сообщение '" + TextToSend.Text + "' на номер " + NumbersToSend.Text + "?", "Отправка сообщения", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        SMSSenderClass sms_obj = new SMSSenderClass();
                        string[] numbers = new string[] { NumbersToSend.Text };
                        var request = new Request { numbers = numbers, text = TextToSend.Text, channel = "DIRECT" };
                        if (sms_obj.sms_send(request) != "false")
                            MessageBox.Show("Успешная отправка сообщения");
                    }
                }
                else
                    MessageBox.Show("Сообщение и/или номер отправления не могут быть пустыми");
            }

            //Отправка произвольного текста всем получателям из БД
            else if ((TextSecondRadioButton.IsChecked == true) && (NumbersFirstRadioButton.IsChecked == true))
            {
                if (TextToSend.Text != "")
                {
                    SMSSenderClass sms_obj = new SMSSenderClass();
                    string bufnumberstr = await DatabaseLogicClass.MySQLGetAsync("SELECT phone FROM Phones");
                    string[] numbers = bufnumberstr.Split(',');
                    Array.Resize(ref numbers, numbers.Length - 1);
                    var request = new Request { numbers = numbers, text = TextToSend.Text, channel = "DIRECT" };
                    if (sms_obj.sms_send(request) != "false")
                        MessageBox.Show("Успешная отправка сообщения всем получателям из БД!");
                }
                else
                    MessageBox.Show("Сообщение для отправки не может быть пустым");
            }

            //Отправка текста из БД произвольному получателю
            else if ((TextFirstRadioButton.IsChecked == true) && (NumbersSecondRadioButton.IsChecked == true) && (GroupsComboBox.SelectedIndex != -1))
            {
                if (NumbersToSend.Text != "")
                {
                    if (SaveDatabaseCheckBox.IsChecked == true)
                    {
                        string thisgroup = GroupsComboBox.SelectedValue.ToString();
                        await DatabaseLogicClass.MySQLExecuteAsync("INSERT INTO Phones(phone,groups) VALUES (\"" + NumbersToSend.Text + "\",\"" + thisgroup + "\");");
                        MessageBox.Show("Успешное добавление нового пользователя с " + thisgroup + " и номером телефона " + NumbersToSend.Text + "!");
                    }

                    if (SendSMSCheckBox.IsChecked == true)
                    {
                        ParseJSONLogicClass thisjson_obj = new ParseJSONLogicClass();
                        thisjson_obj.GetTimetableJSON();
                        string selectedgroup = GroupsComboBox.SelectedValue.ToString();
                        string thisgroupresult = thisjson_obj.GetTimetableByGroup(selectedgroup);

                        SMSSenderClass sms_obj = new SMSSenderClass();
                        if (thisgroupresult == "false")
                        {
                            if (MessageBox.Show("Нет изменений в расписании для группы " + selectedgroup + "\nХотите отправить оповещение об этом?", "Нет изменений", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                string[] numbers = new string[] { NumbersToSend.Text };
                                var request = new Request { numbers = numbers, text = "Нет изменений в расписании группы " + selectedgroup, channel = "DIRECT" };
                                if (sms_obj.sms_send(request) != "false")
                                    MessageBox.Show("Успешная отправка сообщения");
                            }
                        }
                        else
                        {
                            string[] numbers = new string[] { NumbersToSend.Text };
                            var request = new Request { numbers = numbers, text = thisgroupresult, channel = "DIRECT" };
                            if (sms_obj.sms_send(request) != "false")
                                MessageBox.Show("Успешная отправка сообщения");
                        }

                    }
                }
                else
                    MessageBox.Show("Номер отправления не может быть пустым");
                

            }

            //Отправка сообщения из БД получателям из БД 
            else if ((TextFirstRadioButton.IsChecked == true) && (NumbersFirstRadioButton.IsChecked == true))
            {

                ParseJSONLogicClass mainjson_obj = new ParseJSONLogicClass();
                mainjson_obj.GetTimetableJSON();

                string phonesbufresult = await DatabaseLogicClass.MySQLGetAsync("SELECT phone FROM Phones");
                string groupsbufresult = await DatabaseLogicClass.MySQLGetAsync("SELECT groups FROM Phones");

                string[] phonesarr = phonesbufresult.Split(',');
                Array.Resize(ref phonesarr, phonesarr.Length - 1);

                string[] groupsarr = groupsbufresult.Split(',');
                Array.Resize(ref groupsarr, groupsarr.Length - 1);

                int globalcounter = 0;
                for (int i = 0; i < phonesarr.Length; i++) {
                    string thisgroupresult = mainjson_obj.GetTimetableByGroup(groupsarr[i]);
                    if (thisgroupresult != "false")
                    {
                        SMSSenderClass sms_obj = new SMSSenderClass();

                        string[] numbers = new string[] { phonesarr[i] };
                        var request = new Request { numbers = numbers, text = thisgroupresult, channel = "DIRECT" };
                        if (sms_obj.sms_send(request) != "false")
                            globalcounter++;
                        
                    }

                }
                MessageBox.Show("Отправлено " + globalcounter.ToString() + " сообщений из " + phonesarr.Length.ToString());

            }

            GetSMSBalance();
            TextToSend.Text = "";
            NumbersToSend.Text = "";

        }

        private async Task AddGroupListBoxAsync()
        {
            string bufgroupsstr = await DatabaseLogicClass.MySQLGetAsync("SELECT name FROM SMSSender_schema.GroupsTable");
            string[] groupsarr = bufgroupsstr.Split(',');
            for (int i = 0; i< groupsarr.Length-1; i++)
                GroupsComboBox.Items.Add(groupsarr[i]);
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {

            SetColor();
            GetSMSBalance();
            OpenCloseChooseOptions();


            DispatcherTimer SMSTimer = new DispatcherTimer();
            SMSTimer.Tick += new EventHandler(SMSTimer_Tick);
            SMSTimer.Interval = new TimeSpan(0, 0, 0, 30);
            SMSTimer.Start();

            DispatcherTimer ColorTimer = new DispatcherTimer();
            ColorTimer.Tick += new EventHandler(ColorTimer_Tick);
            ColorTimer.Interval = new TimeSpan(0, 0, 0, 3);
            ColorTimer.Start();

            TelegramServerLabel.Content = (TelegramEnabledForm == true) ? "Сервер Telegram: включен": "Сервер Telegram: выключен";
            LoginedNameTextBlock.Text = LocalLoginedName;
            await AddGroupListBoxAsync();


        }

        private void SMSTimer_Tick(object sender, EventArgs e)
        {
            GetSMSBalance();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            SetColor();
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetColor()
        {
            Textgroupbox.BorderBrush = SystemParameters.WindowGlassBrush;
            NumbersGroupBox.BorderBrush = SystemParameters.WindowGlassBrush;
        }

        private void GetSMSBalance()
        {
            try
            {
                SMSSenderClass sms_obj = new SMSSenderClass();
                dynamic BalanceJSON = JObject.Parse(sms_obj.balance());
                BalanceLabel.Content = "Текущий баланс: " + BalanceJSON.data.balance + "₽";
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return;
            }
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
                TG_obj.TelegramInit(2, true);
            }

            else
            {
                DatabaseLogicClass.SQLiteExecute("UPDATE servicetable SET boolvalue = 1 WHERE service='TelegramService'");
                TelegramServerLabel.Content = "Сервер Telegram: включен";

                if (TG_obj == null)
                {
                    TG_obj = new TelegramClass();
                    TG_obj.TelegramInit(1, true);
                }

                TG_obj.TelegramInit(3, true);

            }
           
        }

        private void OpenCloseChooseOptions()
        {

            if ((NumbersSecondRadioButton.IsChecked == true) && (TextFirstRadioButton.IsChecked == true) && (TextSecondRadioButton.IsChecked == false) && NumbersFirstRadioButton.IsChecked == false)
            {

                NumbersGroupBox.Height = 225;
                Textgroupbox.Height = 97;
                NumbersToSend.Visibility = Visibility.Visible;
                TextToSend.Visibility = Visibility.Hidden;
                GroupLabel.Visibility = Visibility.Visible;
                GroupsComboBox.Visibility = Visibility.Visible;
                SaveDatabaseCheckBox.Visibility = Visibility.Visible;
                SendSMSCheckBox.Visibility = Visibility.Visible;

            }

            if ((NumbersSecondRadioButton.IsChecked == false) && (TextFirstRadioButton.IsChecked == true) && (TextSecondRadioButton.IsChecked == false) && NumbersFirstRadioButton.IsChecked == true)
            {
              
                GroupLabel.Visibility = Visibility.Hidden;
                GroupsComboBox.Visibility = Visibility.Hidden;
                SaveDatabaseCheckBox.Visibility = Visibility.Hidden;
                SendSMSCheckBox.Visibility = Visibility.Hidden;
                Textgroupbox.Height = 97;
                NumbersGroupBox.Height = 97;
                TextToSend.Visibility = Visibility.Hidden;
                NumbersToSend.Visibility = Visibility.Hidden;
 
            }
         
            if ((NumbersSecondRadioButton.IsChecked == false) && (TextFirstRadioButton.IsChecked == false) && (TextSecondRadioButton.IsChecked == true) && NumbersFirstRadioButton.IsChecked == true)
            {

                GroupLabel.Visibility = Visibility.Hidden;
                GroupsComboBox.Visibility = Visibility.Hidden;
                SaveDatabaseCheckBox.Visibility = Visibility.Hidden;
                SendSMSCheckBox.Visibility = Visibility.Hidden;
                NumbersGroupBox.Height = 97;
                Textgroupbox.Height = 225;
                NumbersToSend.Visibility = Visibility.Hidden;
                TextToSend.Visibility = Visibility.Visible;

            }

            if ((NumbersSecondRadioButton.IsChecked == true) && (TextFirstRadioButton.IsChecked == false) && (TextSecondRadioButton.IsChecked == true) && NumbersFirstRadioButton.IsChecked == false)
            {

                GroupLabel.Visibility = Visibility.Hidden;
                GroupsComboBox.Visibility = Visibility.Hidden;
                SaveDatabaseCheckBox.Visibility = Visibility.Hidden;
                SendSMSCheckBox.Visibility = Visibility.Hidden;
                NumbersGroupBox.Height = 225;
                Textgroupbox.Height = 225;
                NumbersToSend.Visibility = Visibility.Visible;
                TextToSend.Visibility = Visibility.Visible;

            }

        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagmentWindow UserWindow_obj = new UserManagmentWindow();
            UserWindow_obj.Show();
        }

        private void NumbersSecondRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenCloseChooseOptions();
        }

        private void TextFirstRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenCloseChooseOptions();
        }

        private void TextSecondRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenCloseChooseOptions();
        }

        private void NumbersFirstRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            OpenCloseChooseOptions();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 0, login = '-', pass = '-' WHERE id = 1");
            Close();
        }

    }
}
