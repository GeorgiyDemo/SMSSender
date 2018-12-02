
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        bool TelegramEnabled = false;
        bool ThisAutoLoginEnabled = false;
        TelegramClass TG_obj;
        public MainWindow()
        {
            InitializeComponent();
        }

        private string CheckEmailLogin(string EmailString, string PasswordString, bool Login)
        {
         
            string OutPasswordString;
            if (Login == true)
                OutPasswordString = PasswordString;
            else
                OutPasswordString = CryptoClass.MD5Hash(PasswordString);

            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Email='" + CryptoClass.MD5Hash(EmailString) + "'") == OutPasswordString)
                return DatabaseLogicClass.MySQLGet("SELECT Name FROM Users WHERE Email='" + CryptoClass.MD5Hash(EmailString) + "'");
            return "";
        }

        private string CheckPhoneLogin(string PhoneString, string PasswordString, bool Login)
        {
            string OutPasswordString;
            if (Login == true)
                OutPasswordString = PasswordString;
            else
                OutPasswordString = CryptoClass.MD5Hash(PasswordString);

            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Phone='" + CryptoClass.MD5Hash(PhoneString) + "'") == OutPasswordString)
                return DatabaseLogicClass.MySQLGet("SELECT Name FROM Users WHERE Phone='" + CryptoClass.MD5Hash(PhoneString) + "'");
            return "";
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {

            if ((LoginTextBox.Text != "") && (PasswordBox.Password != ""))
            {
                string CheckPhoneLoginString = CheckPhoneLogin(LoginTextBox.Text, PasswordBox.Password, ThisAutoLoginEnabled);
                string CheckEmailLoginString = CheckEmailLogin(LoginTextBox.Text, PasswordBox.Password, ThisAutoLoginEnabled);
                if ((CheckPhoneLoginString != "") || (CheckEmailLoginString != ""))
                {

                    DatabaseLogicClass.SQLiteExecute("UPDATE logins SET authenticated = 0");
                    DatabaseLogicClass.SQLiteExecute("INSERT INTO logins(login,authenticated) VALUES ('" + CryptoClass.MD5Hash(LoginTextBox.Text) + "',1)");
                    string outnamestr = (CheckPhoneLoginString != "") ? CheckPhoneLoginString : CheckEmailLoginString;
                    SenderWindow SenderWindow_obj = new SenderWindow(TG_obj, TelegramEnabled, outnamestr);

                    if ((SaveLoginCheckBox.IsChecked == true) && (ThisAutoLoginEnabled == false))
                        DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 1, login = '" + LoginTextBox.Text + "', pass = '" + CryptoClass.MD5Hash(PasswordBox.Password) + "' WHERE id = 1");
                    else if (SaveLoginCheckBox.IsChecked == false)
                        DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 0, login = '-', pass = '-' WHERE id = 1");

                    MessageBox.Show("Успешная авторизация");
                    SenderWindow_obj.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка аутентификации");
                    DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 0, login = '-', pass = '-' WHERE id = 1");
                    ThisAutoLoginEnabled = false;
                    LoginTextBox.Text = "";
                    PasswordBox.Password = "";
                }
            }
            else
                MessageBox.Show("Введите логин пользователя и пароль");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (PingClass.CheckForInternetConnection() == false)
            {
                MessageBox.Show("Проверьте подключение и повторите попытку");
                Close();
            }
            else
            {
                SetColor();
                DispatcherTimer ColorTimer = new DispatcherTimer();
                ColorTimer.Tick += new EventHandler(ColorTimer_Tick);
                ColorTimer.Interval = new TimeSpan(0, 0, 0, 3);
                ColorTimer.Start();

                TG_obj = new TelegramClass();
                if (DatabaseLogicClass.SQLiteGet("SELECT boolvalue FROM servicetable WHERE service='TelegramService'") == "1")
                {
                    TG_obj.TelegramInit(1, true);
                    TelegramEnabled = true;
                }
                else if (DatabaseLogicClass.SQLiteGet("SELECT boolvalue FROM servicetable WHERE service='TelegramService'") == "0")
                {
                    TG_obj.TelegramInit(1,false);
                    TelegramEnabled = false;
                    TG_obj.TelegramInit(2,false);
                }

                if (DatabaseLogicClass.SQLiteGet("SELECT savedbool FROM savedlogin WHERE id=1") == "1")
                {
                    LoginTextBox.Text = DatabaseLogicClass.SQLiteGet("SELECT login FROM savedlogin WHERE id=1");
                    PasswordBox.Password = DatabaseLogicClass.SQLiteGet("SELECT pass FROM savedlogin WHERE id=1");
                    ThisAutoLoginEnabled = true;
                    SaveLoginCheckBox.IsChecked = true;
                }
            }

        }
        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            SetColor();
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetColor()
        {
            LoginGroupBox.BorderBrush = SystemParameters.WindowGlassBrush;
        }

    }
}
