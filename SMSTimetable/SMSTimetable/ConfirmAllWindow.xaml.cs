﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для ConfirmAllWindow.xaml
    /// </summary>
    public partial class ConfirmAllWindow : Window
    {

        private static string EmailString, SMSString, EmailCode, SMSCode, UserPassword, salt, username;
        private static bool ValidSMSCode, ValidEmailCode = false;
        public ConfirmAllWindow(string Email, string SMS, string UPassword, string inputsalt, string InputUsername)
        {
            EmailString = Email;
            SMSString = SMS;
            UserPassword = UPassword;
            salt = inputsalt;
            username = InputUsername;
            InitializeComponent();
        }
        private async void FinalConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            await DatabaseLogicClass.MySQLExecuteAsync("INSERT INTO Users(Phone,Email,Password,Name) VALUES ('" + CryptoClass.MD5Hash(SMSString) + "','" + CryptoClass.MD5Hash(EmailString) + "','" + CryptoClass.MD5Hash(UserPassword) + "','"+CryptoClass.Base64Encode(username) +"')");
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

        private void Window_Initialized(object sender, EventArgs e)
        {
            FinalConfirmButton.IsEnabled = false;
            EmailCodeLabel.Content = "Введите код, отправленный на e-mail " + EmailString + ":";
            PhoneCodeLabel.Content = "Введите код, отправленный на номер " + SMSString + ":";

            EmailCode = DatabaseLogicClass.SQLiteGet("SELECT code FROM codes WHERE code_source='"+CryptoClass.MD5Hash(EmailString+salt) +"'");
            SMSCode = DatabaseLogicClass.SQLiteGet("SELECT code FROM codes WHERE code_source='" + CryptoClass.MD5Hash(SMSString+salt) +"'");

        }
    }
}
