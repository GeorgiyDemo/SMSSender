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
    /// Логика взаимодействия для UserChangePasswordWindow.xaml
    /// </summary>
    public partial class UserChangePasswordWindow : Window
    {
        bool ValidOldEmail, ValidNewPassword, ValidNewRepeatEmail = false;
        public UserChangePasswordWindow()
        {
            InitializeComponent();
        }


        private async Task<bool> ValidOldPassword(string OldPassword)
        {
            if (ValidatorClass.ValidatePassword(OldPassword) == true)
            {

                string MD5Login = DatabaseLogicClass.SQLiteGet("SELECT login FROM logins WHERE authenticated=1");

                string result = await DatabaseLogicClass.MySQLGetAsync("SELECT Password FROM Users WHERE (Phone='" + MD5Login + "' OR Email='" + MD5Login + "')");
                result = result.Remove(result.Length - 1);


                if (CryptoClass.MD5Hash(OldPassword) == result)
                {
                    OldPasswordComments.Content = "-> верный пароль";
                    return true;
                }

                else
                    OldPasswordComments.Content = "-> неверный пароль";

            }
            return false;

        }

        private void EmailConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программируем");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OldPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool OldPasswordResult = await ValidOldPassword(OldPasswordTextBox.Text);
            if (OldPasswordResult == true)
            {
                ValidOldEmail = true;
            }
            else
            {
                ValidOldEmail = false;
            }

            if ((ValidOldEmail == true) && (ValidNewPassword == true) && (ValidNewRepeatEmail == true))
                PasswordConfirmButton.IsEnabled = true;
            else
                PasswordConfirmButton.IsEnabled = false;
        }

        private void NewPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidatorClass.ValidatePassword(NewPasswordTextBox.Text) == true)
            {
                ValidNewPassword = true;
                NewPasswordComments.Content = "-> хороший пароль";
            }
            else
            {
                ValidNewPassword = false;
                NewPasswordComments.Content = "-> слабый пароль";
            }
        }

        private void NewRepeatPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
 
        }
    }
}
