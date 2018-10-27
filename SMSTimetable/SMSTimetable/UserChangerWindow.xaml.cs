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
    /// Логика взаимодействия для UserChangerWindow.xaml
    /// </summary>
    public partial class UserChangerWindow : Window
    {
        public UserChangerWindow()
        {
            InitializeComponent();
        }

        private void ChangeEmailButton_Click(object sender, RoutedEventArgs e)
        {
            UserChangeEmailWindow EmailChanger_obj = new UserChangeEmailWindow();
            EmailChanger_obj.Show();
            Close();
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            UserChangePasswordWindow UserChangePassword_obj = new UserChangePasswordWindow();
            UserChangePassword_obj.Show();
            Close();
        }

        private void ChangePhoneButton_Click(object sender, RoutedEventArgs e)
        {
            UserChangePhoneWindow userChangePhone_obj = new UserChangePhoneWindow();
            userChangePhone_obj.Show();
            Close();
        }
    }
}
