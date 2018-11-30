using System.Windows;

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

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            UserRemoveWindow UserRemove_obj = new UserRemoveWindow();
            UserRemove_obj.Show();
            Close();
        }
    }
}
