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
using System.Windows.Threading;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для UserChangeEmailWindow.xaml
    /// </summary>
    public partial class UserChangeEmailWindow : Window
    {
        bool OldValidEmail, NewValidEmail = false;

        public UserChangeEmailWindow()
        {
            InitializeComponent();
        }

        private async Task<bool> ValidOldEmail(string OldEmailText)
        {
            if (ValidatorClass.IsValidEmail(OldEmailText) == true)
            {

                string MD5Login = DatabaseLogicClass.SQLiteGet("SELECT login FROM logins WHERE authenticated=1");

                string result = await DatabaseLogicClass.MySQLGetAsync("SELECT Email FROM Users WHERE (Phone='" + MD5Login + "' OR Email='"+ MD5Login + "')");
                result = result.Remove(result.Length - 1);

                if (CryptoClass.MD5Hash(OldEmailText) == MD5Login)
                    return true;
                else if (CryptoClass.MD5Hash(OldEmailText) == result)
                    return true;
                else
                    OldEmailComments.Content = "-> не ваш e-mail или его не существует";

            }

            else
                OldEmailComments.Content = "-> некорректный e-mail";

            return false;
        }

        private async void EmailConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string EmailCode = CryptoClass.GetRandomNumber();
            await EmailSenderClass.SendEmailAsync("Ваш код для подтверждения e-mail: " + EmailCode, NewEmailTextBox.Text);
            DEMKAInputBox demka_obj = new DEMKAInputBox("Ввведите код, отправленный на новый e-mail");
            string InputCode = demka_obj.ShowDialog();

            if (InputCode == EmailCode)
            {
                await DatabaseLogicClass.MySQLExecuteAsync("UPDATE Users SET Email = '"+CryptoClass.MD5Hash(NewEmailTextBox.Text) +"' WHERE Email='"+CryptoClass.MD5Hash(OldEmailTextBox.Text) +"' ");
                MessageBox.Show("Успешно обновили email с "+OldEmailTextBox.Text+" на "+NewEmailTextBox.Text);
                Close();
            }

        }

        private void NewEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.IsValidEmail(NewEmailTextBox.Text) == true)
            {
                NewEmailComments.Content = "-> корректный e-mail";
                NewValidEmail = true;
            }
            else
            {
                NewEmailComments.Content = "-> некорректный e-mail";
                NewValidEmail = false;
            }

            EmailConfirmButton.IsEnabled = (NewValidEmail == true) && (OldValidEmail == true);

        }

        private async void OldEmailTextBox_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            bool Validated = await ValidOldEmail(OldEmailTextBox.Text);
            if (Validated == true)
            {
                OldEmailComments.Content = "-> корректный e-mail";
                OldValidEmail = true;
            }
            else
                OldValidEmail = false;

            EmailConfirmButton.IsEnabled = (NewValidEmail == true) && (OldValidEmail == true);
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
           
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
