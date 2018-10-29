using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для UserAddWindow.xaml
    /// </summary>
    public partial class UserAddWindow : Window
    {
        bool ValidUsername, ValidEmail, ValidPhone, ValidPassword, ValidMasterPassword = false;

        public UserAddWindow()
        {
            InitializeComponent();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if ((ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true))
            {
                string salt = CryptoClass.GetRandomNumber();
                //Отправка Email
                string EmailCode = CryptoClass.GetRandomNumber();
                DatabaseLogicClass.SQLiteExecute("INSERT INTO codes(code_source,code) VALUES ('" + CryptoClass.MD5Hash(EmalTextBox.Text+salt) + "','"+CryptoClass.MD5Hash(EmailCode)+"')");
                
                await EmailSenderClass.SendEmailAsync("Ваш код для подтверждения e-mail: " + EmailCode,EmalTextBox.Text);
                //MessageBox.Show("Email code: " + EmailCode);

                //Отпрвка SMS
                string SMSCode = CryptoClass.GetRandomNumber();
                DatabaseLogicClass.SQLiteExecute("INSERT INTO codes(code_source,code) VALUES ('" + CryptoClass.MD5Hash(PhoneTextBox.Text+salt) + "','" + CryptoClass.MD5Hash(SMSCode) + "')");
                SMSSenderClass ConfirmSMS_obj = new SMSSenderClass();
                string[] numbers = new string[] { PhoneTextBox.Text };
                var request = new Request { numbers = numbers, text = SMSCode, channel = "DIRECT" };

                ConfirmSMS_obj.sms_send(request);
                //MessageBox.Show("Тук тук халявная SMS: "+SMSCode);

                ConfirmAllWindow ConfirmWindow_obj = new ConfirmAllWindow(EmalTextBox.Text,PhoneTextBox.Text, CryptoClass.MD5Hash(PasswordBox.Password), salt, UsernameTextBox.Text);
                ConfirmWindow_obj.Show();
                Close();
            }

        }

        private async void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            if (ValidatorClass.IsPhoneNumber(PhoneTextBox.Text) == true)
            {
                string PhoneCheckAsync = await DatabaseLogicClass.MySQLGetAsync("SELECT Password FROM Users WHERE Phone='" +CryptoClass.MD5Hash(PhoneTextBox.Text)+ "'");

                if (PhoneCheckAsync == "")
                {
                    Phonecomments.Content = "-> валидный телефон";
                    PhoneTextBox.Foreground = Brushes.Black;
                    ValidPhone = true;
                }

                else
                {
                    Phonecomments.Content = "-> телефон уже зарегистрирован!";
                    ValidPhone = false;
                }
            }
            else
            {
                Phonecomments.Content = "-> некорректный телефон";
                PhoneTextBox.Foreground = Brushes.Red;
                ValidPhone = false;
            }

            NextButton.IsEnabled = (ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true);

        }

        private async void EmalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.IsValidEmail(EmalTextBox.Text) == true)
            {
                string EmailCheckAsync = await DatabaseLogicClass.MySQLGetAsync("SELECT Password FROM Users WHERE Email='" + CryptoClass.MD5Hash(EmalTextBox.Text) + "'");

                if (EmailCheckAsync == "")
                {
                    Emailcomments.Content = "-> валидный e-mail";
                    EmalTextBox.Foreground = Brushes.Black;
                    ValidEmail = true;
                }
                else
                {
                    Emailcomments.Content = "-> e-mail уже зарегистрирован!";
                    EmalTextBox.Foreground = Brushes.Black;
                    ValidEmail = false;
                }
            }

            else
            {
                Emailcomments.Content = "-> некорректный e-mail";
                EmalTextBox.Foreground = Brushes.Red;
                ValidEmail = false;
            }

            NextButton.IsEnabled = (ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true);

        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            SetColor();
            DispatcherTimer ColorTimer = new DispatcherTimer();
            ColorTimer.Tick += new EventHandler(ColorTimer_Tick);
            ColorTimer.Interval = new TimeSpan(0, 0, 0, 3);
            ColorTimer.Start();
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            SetColor();
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetColor()
        {
            AddGroupBox.BorderBrush = SystemParameters.WindowGlassBrush;
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameTextBox.Text != ""){
                ValidUsername = true;
                Usernamecomments.Content = "-> корректное имя";
            }
            else
            {
                ValidUsername = false;
                Usernamecomments.Content = "-> некорректное имя";
            }
            
            NextButton.IsEnabled = (ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ValidatorClass.ValidatePassword(PasswordBox.Password) == true)
            {
                Passwordcomments.Content = "-> хороший пароль";
                ValidPassword = true;
            }
            else
            {
                Passwordcomments.Content = "-> слабый пароль";
                ValidPassword = false;
            }

            NextButton.IsEnabled = (ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true);

        }

        private void MasterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ValidatorClass.ValidateMasterPassword(MasterPasswordBox.Password) == true)
            {
                MasterPasswordcomments.Content = "-> валидный";
                ValidMasterPassword = true;
            }
            else
            {
                MasterPasswordcomments.Content = "-> невалидный";
                ValidMasterPassword = false;
            }

            NextButton.IsEnabled = (ValidUsername == true) && (ValidEmail == true) && (ValidPhone == true) && (ValidPassword == true) && (ValidMasterPassword == true);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
