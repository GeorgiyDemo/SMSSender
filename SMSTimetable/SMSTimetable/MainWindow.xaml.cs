
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

        private bool CheckEmailLogin(string EmailString, string PasswordString, bool Login)
        {
         
            string OutPasswordString;
            if (Login == true)
                OutPasswordString = PasswordString;
            else
                OutPasswordString = CryptoClass.MD5Hash(PasswordString);
             
            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Email='"+CryptoClass.MD5Hash(EmailString) +"'") == OutPasswordString)
                return true;
            return false;
        }

        private bool CheckPhoneLogin(string PhoneString, string PasswordString, bool Login)
        {
            string OutPasswordString;
            if (Login == true)
                OutPasswordString = PasswordString;
            else
                OutPasswordString = CryptoClass.MD5Hash(PasswordString);

            if (DatabaseLogicClass.MySQLGet("SELECT Password FROM Users WHERE Phone='" + CryptoClass.MD5Hash(PhoneString) + "'") == OutPasswordString)
                return true;
            return false;
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {

            if ((LoginTextBox.Text != "") && (PasswordBox.Password != ""))
            {
                if ((CheckPhoneLogin(LoginTextBox.Text, PasswordBox.Password, ThisAutoLoginEnabled) == true) || (CheckEmailLogin(LoginTextBox.Text, PasswordBox.Password, ThisAutoLoginEnabled) == true))
                {
   
                    DatabaseLogicClass.SQLiteExecute("UPDATE logins SET authenticated = 0");
                    DatabaseLogicClass.SQLiteExecute("INSERT INTO logins(login,authenticated) VALUES ('" + CryptoClass.MD5Hash(LoginTextBox.Text) + "',1)");
                    SenderWindow SenderWindow_obj = new SenderWindow(TG_obj, TelegramEnabled);

                    if ((SaveLoginCheckBox.IsChecked == true) && (ThisAutoLoginEnabled == false))
                        DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 1, login = '" + LoginTextBox.Text + "', pass = '" + CryptoClass.MD5Hash(PasswordBox.Password) + "' WHERE id = 1");
                    else if (SaveLoginCheckBox.IsChecked == false)
                        DatabaseLogicClass.SQLiteExecute("UPDATE savedlogin SET savedbool = 0, login = '-', pass = '-' WHERE id = 1");

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
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SetColor();
            DispatcherTimer ColorTimer = new DispatcherTimer();
            ColorTimer.Tick += new EventHandler(ColorTimer_Tick);
            ColorTimer.Interval = new TimeSpan(0, 0, 0, 3);
            ColorTimer.Start();

            TG_obj = new TelegramClass();

            if (DatabaseLogicClass.SQLiteGet("SELECT boolvalue FROM servicetable WHERE service='TelegramService'") == "1")
            {
                TelegramEnabled = true;
                TG_obj.TelegramInit(1);
            }

            if (DatabaseLogicClass.SQLiteGet("SELECT savedbool FROM savedlogin WHERE id=1") == "1")
            {
                LoginTextBox.Text = DatabaseLogicClass.SQLiteGet("SELECT login FROM savedlogin WHERE id=1");
                PasswordBox.Password = DatabaseLogicClass.SQLiteGet("SELECT pass FROM savedlogin WHERE id=1");
                ThisAutoLoginEnabled = true;
                SaveLoginCheckBox.IsChecked = true;
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
