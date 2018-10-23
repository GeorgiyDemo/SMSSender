using System;
using System.Windows;

namespace SMSTimetable
{
    /// <summary>
    /// Логика взаимодействия для SMSStatusWindow.xaml
    /// </summary>
    public partial class SMSStatusWindow : Window
    {
        public SMSStatusWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SMSStatusClass SMSStatus_obj = new SMSStatusClass();
            SMSStatusDataGrid.ItemsSource = SMSStatus_obj.DataGridInput().DefaultView;
        }
    }
}


