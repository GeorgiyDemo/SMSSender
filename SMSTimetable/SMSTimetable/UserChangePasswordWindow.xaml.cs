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
        bool ValidOldPassword, ValidNewPassword, ValidNewRepeatPassword = false;
        public UserChangePasswordWindow()
        {
            InitializeComponent();
        }


        private async Task<bool> ValidationOldPassword(string OldPassword)
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
            if ((ValidOldPassword == true) && (ValidNewPassword == true) && (ValidNewRepeatPassword == true))
                MessageBox.Show("Программируем");
            else
                MessageBox.Show("Не программируем");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OldPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool OldPasswordResult = await ValidationOldPassword(OldPassword.Password);
            ValidOldPassword = (OldPasswordResult == true) ? true : false;

            PasswordConfirmButton.IsEnabled = (ValidOldPassword == true) && (ValidNewPassword == true) && (ValidNewRepeatPassword == true);

        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ValidatorClass.ValidatePassword(NewPassword.Password) == true)
            {
                ValidNewPassword = true;
                NewPasswordComments.Content = "-> хороший пароль";
            }
            else
            {
                ValidNewPassword = false;
                NewPasswordComments.Content = "-> слабый пароль";
            }

            PasswordConfirmButton.IsEnabled = (ValidOldPassword == true) && (ValidNewPassword == true) && (ValidNewRepeatPassword == true);

        }
        private void NewRepeatPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (NewPassword.Password == NewRepeatPassword.Password)
            {
                ValidNewRepeatPassword = true;
                NewRepeatPasswordComments.Content = "-> пароли совпадают";
            }
            else
            {
                ValidNewRepeatPassword = false;
                NewRepeatPasswordComments.Content = "-> пароли не совпадают";
            }

            PasswordConfirmButton.IsEnabled = (ValidOldPassword == true) && (ValidNewPassword == true) && (ValidNewRepeatPassword == true);

        }
    }
}
