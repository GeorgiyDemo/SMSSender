using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для UserAddWindow.xaml
    /// </summary>
    public partial class UserAddWindow : Window
    {
        bool ValidEmail, ValidPhone, ValidPassword, ValidMasterPassword = false;

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
          
            if (ValidEmail == ValidPhone == ValidPassword == ValidMasterPassword == true)
            {
                //Отправка Email
                string EmailCode = CryptoClass.GetRandomNumber();
                DatabaseLogicClass.SQLiteExecute("INSERT INTO codes(code_source,code) VALUES ('" + CryptoClass.MD5Hash(EmalTextBox.Text) + "','"+CryptoClass.MD5Hash(EmailCode)+"')");
                
                //await EmailSenderClass.SendEmailAsync("Ваш код для подтверждения e-mail: " + EmailCode,EmalTextBox.Text);
                ////////
                MessageBox.Show("Email code: " + EmailCode);
                ////////

                //Отпрвка SMS
                string SMSCode = CryptoClass.GetRandomNumber();
                DatabaseLogicClass.SQLiteExecute("INSERT INTO codes(code_source,code) VALUES ('" + CryptoClass.MD5Hash(PhoneTextBox.Text) + "','" + CryptoClass.MD5Hash(SMSCode) + "')");
                SMSSenderClass ConfirmSMS_obj = new SMSSenderClass();
                string[] numbers = new string[] { PhoneTextBox.Text };
                var request = new Request { numbers = numbers, text = SMSCode, channel = "DIRECT" };

                //ConfirmSMS_obj.sms_send(request);
                ////////////
                MessageBox.Show("Тук тук халявная SMS: "+SMSCode);
                ////////////

                ConfirmAllWindow ConfirmWindow_obj = new ConfirmAllWindow(EmalTextBox.Text,PhoneTextBox.Text, CryptoClass.MD5Hash(PasswordBox.Password));
                ConfirmWindow_obj.Show();
                Close();
            }

        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.IsPhoneNumber(PhoneTextBox.Text) == true)
            {
                Phonecomments.Content = "-> валидный телефон";
                PhoneTextBox.Foreground = Brushes.Black;
                ValidPhone = true;
            }
            else
            {
                Phonecomments.Content = "-> некорректный телефон";
                PhoneTextBox.Foreground = Brushes.Red;
                ValidPhone = false;
            }
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
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        }

        public UserAddWindow()
        {
            InitializeComponent();
        }

        private void EmalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.IsValidEmail(EmalTextBox.Text) == true)
            {
                Emailcomments.Content = "-> валидный e-mail";
                EmalTextBox.Foreground = Brushes.Black;
                ValidEmail = true;
            }

            else
            {
                Emailcomments.Content = "-> некорректный e-mail";
                EmalTextBox.Foreground = Brushes.Red;
                ValidEmail = false;
            }
        }
    }
}
