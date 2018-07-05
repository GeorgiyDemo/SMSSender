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
    /// Логика взаимодействия для SenderWindow.xaml
    /// </summary>
    public partial class SenderWindow : Window
    {
        public SenderWindow()
        {
            InitializeComponent();
        }

        private void TextSecondRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FinalSendButton_Click(object sender, RoutedEventArgs e)
        {
           SMSSenderClass sms_obj = new SMSSenderClass();
            string[] numbers = new string[] { NumbersToSend.Text };
            var request = new Request { numbers = numbers, text = TextToSend.Text, channel = "DIRECT" };
           sms_obj.sms_send(request);
        }
    }
}
