
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

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        bool TelegramEnabled = false;
        TelegramClass TG_obj;
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool CheckEmailLogin(string EmailString, string PasswordString)
        {
            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Email='"+CryptoClass.MD5Hash(EmailString) +"'") == CryptoClass.MD5Hash(PasswordString))
                return true;
            return false;
        }

        private bool CheckPhoneLogin(string PhoneString, string PasswordString)
        {
            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Phone='" + CryptoClass.MD5Hash(PhoneString) + "'") == CryptoClass.MD5Hash(PasswordString))
                return true;
            return false;
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if ((LoginTextBox.Text != "") && (PasswordBox.Password != ""))
            {
                if ((CheckPhoneLogin(LoginTextBox.Text, PasswordBox.Password) == true) || (CheckEmailLogin(LoginTextBox.Text, PasswordBox.Password) == true))
                {
                    DatabaseLogicClass.SQLiteExecute("UPDATE logins SET authenticated = 0");
                    DatabaseLogicClass.SQLiteExecute("INSERT INTO logins(login,authenticated) VALUES ('" + CryptoClass.MD5Hash(LoginTextBox.Text) + "',1)");
                    SenderWindow SenderWindow_obj = new SenderWindow(TG_obj, TelegramEnabled);
                    SenderWindow_obj.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка аутентификации");
                    LoginTextBox.Text = "";
                    PasswordBox.Password = "";
                }
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            TG_obj = new TelegramClass();

            if (DatabaseLogicClass.SQLiteGet("SELECT boolvalue FROM servicetable WHERE service='TelegramService'") == "1")
            {
                TelegramEnabled = true;
                TG_obj.TelegramInit(1);
            }

        }
    }
}
