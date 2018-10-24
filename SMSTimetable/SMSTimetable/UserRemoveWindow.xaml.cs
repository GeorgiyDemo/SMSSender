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
    /// Логика взаимодействия для UserRemoveWindow.xaml
    /// </summary>
    public partial class UserRemoveWindow : Window
    {
        bool ValidLogin, ValidMasterPassword = false;
        public UserRemoveWindow()
        {
            InitializeComponent();
        }

        private static async Task<bool> CheckUserLogin(string InputLogin)
        {
            string ThisResult = await DatabaseLogicClass.MySQLGetAsync("SELECT Password FROM Users WHERE Email='" + CryptoClass.MD5Hash(InputLogin) + "' OR Phone='"+ CryptoClass.MD5Hash(InputLogin) + "'");
            if (ThisResult != "")
                return true;
            return false;
        }

        private async void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((ValidatorClass.IsPhoneNumber(LoginTextBox.Text) == true) || (ValidatorClass.IsValidEmail(LoginTextBox.Text) == true))
            {
                bool FlagResult = await CheckUserLogin(LoginTextBox.Text);
                if (FlagResult == true)
                {
                    Logincomments.Content = "-> валидный";
                    ValidLogin = true;
                }
                else
                {
                    Logincomments.Content = "-> невалидный";
                    ValidLogin = false;
                }
            }
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

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if ((ValidLogin == true) && (ValidMasterPassword == true))
            {
                await DatabaseLogicClass.MySQLExecuteAsync("DELETE FROM Users WHERE (Phone = '" + CryptoClass.MD5Hash(LoginTextBox.Text) + "' OR Email =  '" + CryptoClass.MD5Hash(LoginTextBox.Text) + "');");
                MessageBox.Show("Пользователь с логином " + LoginTextBox.Text + " был успешно удален из системы");
                Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
