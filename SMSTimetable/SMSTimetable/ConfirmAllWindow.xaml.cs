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
    /// Логика взаимодействия для ConfirmAllWindow.xaml
    /// </summary>
    public partial class ConfirmAllWindow : Window
    {

        private static string EmailString, SMSString, EmailCode, SMSCode, UserPassword, salt;
        private static bool ValidSMSCode, ValidEmailCode = false;

        private async void FinalConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            await DatabaseLogicClass.MySQLExecuteAsync("INSERT INTO Users(Phone,Email,Password) VALUES ('" + CryptoClass.MD5Hash(SMSString) + "','" + CryptoClass.MD5Hash(EmailString) + "','" + CryptoClass.MD5Hash(UserPassword) + "')");
            MessageBox.Show("Новый пользователь с e-mail " + EmailString + " и телефоном " + SMSString + " был успешно добавлен в систему");
            Close();
        }


        private void EmalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (CryptoClass.MD5Hash(EmalTextBox.Text) == EmailCode)
            {
                EmailComments.Content = "-> верный код";
                ValidEmailCode = true;
            }
            else
            {
                EmailComments.Content = "-> неверный код";
                ValidEmailCode = false;
            }

            if ((ValidEmailCode == true) && (ValidSMSCode == true))
                FinalConfirmButton.IsEnabled = true;
            else
                FinalConfirmButton.IsEnabled = false;

        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MessageBox.Show(CryptoClass.MD5Hash(PhoneTextBox.Text));
            MessageBox.Show("SMSCODE FROM BD:" + SMSCode);
            if (CryptoClass.MD5Hash(PhoneTextBox.Text) == SMSCode)
            {
                PhoneComments.Content = "-> верный код";
                ValidSMSCode = true;
            }

            else
            {
                PhoneComments.Content = "-> неверный код";
                ValidSMSCode = false;
            }

            if ((ValidEmailCode == true) && (ValidSMSCode == true))
                FinalConfirmButton.IsEnabled = true;
            else
                FinalConfirmButton.IsEnabled = false;
        }

        public ConfirmAllWindow(string Email, string SMS, string UPassword, string inputsalt) 
        {
            EmailString = Email;
            SMSString = SMS;
            UserPassword = UPassword;
            salt = inputsalt;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            FinalConfirmButton.IsEnabled = false;
            EmailCodeLabel.Content = "Введите код, отправленный на e-mail " + EmailString;
            PhoneCodeLabel.Content = "Введите код, отправленный на " + SMSString;

            EmailCode = DatabaseLogicClass.SQLiteGet("SELECT code FROM codes WHERE code_source='"+CryptoClass.MD5Hash(EmailString+salt) +"'");
            SMSCode = DatabaseLogicClass.SQLiteGet("SELECT code FROM codes WHERE code_source='" + CryptoClass.MD5Hash(SMSString+salt) +"'");

        }
    }
}
