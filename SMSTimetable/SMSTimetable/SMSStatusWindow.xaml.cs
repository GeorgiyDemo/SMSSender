using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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
            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SMSStatusClass SMSStatus_obj = new SMSStatusClass();
            SMSStatusDataGrid.ItemsSource = SMSStatus_obj.DataGridInput().DefaultView;

            //DataGridTableSMSStatus;
        }
    }
}


