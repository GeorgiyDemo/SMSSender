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
    /// Логика взаимодействия для UserManagmentWindow.xaml
    /// </summary>
    public partial class UserManagmentWindow : Window
    {
        public UserManagmentWindow()
        {
            InitializeComponent();
        }

        private void ChangeThisUserButton_Click(object sender, RoutedEventArgs e)
        {
            UserChangerWindow UserChanger_obj = new UserChangerWindow();
            UserChanger_obj.Show();
            Close();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            UserAddWindow UserAdd_obj = new UserAddWindow();
            UserAdd_obj.Show();
            Close();
        }
    }
}
