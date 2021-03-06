﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
                    LoginTextBox.Foreground = Brushes.Black;
                    Logincomments.Content = "-> валидный";
                    ValidLogin = true;
                }
                else
                {
                    LoginTextBox.Foreground = Brushes.Red;
                    Logincomments.Content = "-> невалидный";
                    ValidLogin = false;
                }
            }
            else
            {
                LoginTextBox.Foreground = Brushes.Red;
                Logincomments.Content = "-> невалидный";
                ValidLogin = false;
            }

            NextButton.IsEnabled = (ValidLogin == true) && (ValidMasterPassword == true);
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

            NextButton.IsEnabled = (ValidLogin == true) && (ValidMasterPassword == true);
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

        private void Window_Initialized(object sender, EventArgs e)
        {
            SetColor();
            DispatcherTimer ColorTimer = new DispatcherTimer();
            ColorTimer.Tick += new EventHandler(ColorTimer_Tick);
            ColorTimer.Interval = new TimeSpan(0, 0, 0, 3);
            ColorTimer.Start();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            SetColor();
            CommandManager.InvalidateRequerySuggested();
        }

        private void SetColor()
        {
            RemoveGroupBox.BorderBrush = SystemParameters.WindowGlassBrush;
        }

    }
}
