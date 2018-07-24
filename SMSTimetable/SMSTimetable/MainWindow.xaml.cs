
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            DEMKAInputBox demka_obj = new DEMKAInputBox("MEOW");
            string test = demka_obj.ShowDialog();
            MessageBox.Show(test);


            //string test = new DEMKAInputBox.DEMKAInputBox("text");
            SenderWindow SenderWindow_obj = new SenderWindow();
            SenderWindow_obj.Show();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            TelegramClass.TelegramInit();
        }
    }
}
