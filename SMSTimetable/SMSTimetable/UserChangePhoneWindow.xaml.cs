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
using System.Windows.Shapes;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для UserChangePhoneWindow.xaml
    /// </summary>
    public partial class UserChangePhoneWindow : Window
    {
        bool OldPhoneValidation = false;
        bool NewPhoneValidation = false;
        public UserChangePhoneWindow()
        {
            InitializeComponent();
        }

        private async Task<bool> ValidationOldPhone(string OldPhone)
        {
            if (ValidatorClass.IsPhoneNumber(OldPhone) == true)
            {

                string MD5Login = DatabaseLogicClass.SQLiteGet("SELECT login FROM logins WHERE authenticated=1");

                string result = await DatabaseLogicClass.MySQLGetAsync("SELECT Phone FROM Users WHERE (Phone='" + MD5Login + "' OR Email='" + MD5Login + "')");
                result = result.Remove(result.Length - 1);


                return CryptoClass.MD5Hash(OldPhone) == result;


            }
            return false;

        }

        private void NewPhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.IsPhoneNumber(NewPhoneTextBox.Text) == true)
            {   
                NewPhoneComments.Content = "-> верный телефон";
                NewPhoneTextBox.Foreground = Brushes.Black;
                NewPhoneValidation = true;
            }
            else
            {
                NewPhoneComments.Content = "-> неверный телефон";
                NewPhoneTextBox.Foreground = Brushes.Red;
                NewPhoneValidation = false;
            }

            PhoneConfirmButton.IsEnabled = (OldPhoneValidation = true) && (NewPhoneValidation = true);
        }

        private async void OldPhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool bufresult = await ValidationOldPhone(OldPhoneTextBox.Text);
            if (bufresult == true)
            {
                OldPhoneComments.Content = "-> верный телефон";
                OldPhoneTextBox.Foreground = Brushes.Black;
                OldPhoneValidation = true;
            }

            else
            {
                OldPhoneComments.Content = "-> неверный телефон";
                OldPhoneTextBox.Foreground = Brushes.Red;
                OldPhoneValidation = false;
            }

            PhoneConfirmButton.IsEnabled = (OldPhoneValidation = true) && (NewPhoneValidation = true);
        }

        private async void PhoneConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if ((OldPhoneValidation == true) && (NewPhoneValidation = true))
            {
                MessageBox.Show("ВОШЛИ В УСЛОВИЕ");
                string salt = CryptoClass.GetRandomNumber();
                string SMSCode = CryptoClass.GetRandomNumber();
                DatabaseLogicClass.SQLiteExecute("INSERT INTO codes(code_source,code) VALUES ('" + CryptoClass.MD5Hash(NewPhoneTextBox.Text + salt) + "','" + CryptoClass.MD5Hash(SMSCode) + "')");
                SMSSenderClass ConfirmSMS_obj = new SMSSenderClass();
                string[] numbers = new string[] { NewPhoneTextBox.Text };
                var request = new Request { numbers = numbers, text = SMSCode, channel = "DIRECT" };
                ConfirmSMS_obj.sms_send(request);

                DEMKAInputBox SMSdemka_obj = new DEMKAInputBox("Ввведите код, отправленный на новый номер телефона");
                string InputCode = SMSdemka_obj.ShowDialog();

                if (InputCode == SMSCode)
                {
                    await DatabaseLogicClass.MySQLExecuteAsync("UPDATE Users SET Phone = '" + CryptoClass.MD5Hash(NewPhoneTextBox.Text) + "' WHERE Phone='" + CryptoClass.MD5Hash(OldPhoneTextBox.Text) + "' ");
                    MessageBox.Show("Успешно обновили телефон с " + OldPhoneTextBox.Text + " на " + NewPhoneTextBox.Text);
                    Close();
                }
                else
                    MessageBox.Show("Неверный код, попробуйте снова");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
