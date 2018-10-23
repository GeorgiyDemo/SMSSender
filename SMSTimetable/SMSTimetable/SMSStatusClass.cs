using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMSTimetable
{
    class SMSStatusClass
    {

        public string[,] GetSMSInfoAPI()
        {


            SMSSenderClass SMSHistory_obj = new SMSSenderClass();
            dynamic HistoryJSON = JObject.Parse(SMSHistory_obj.sms_list(new Request { page = 1 }));
            int length = HistoryJSON.data.Count;
            MessageBox.Show(length.ToString());
            string[,] array = new string[5, 5];
            for (int i = 0; i < 5; i++)
            {
                JObject ThisSMS = HistoryJSON.data[i.ToString()];
                array[i, 0] = (i + 1).ToString();
                array[i, 1] = ThisSMS["number"].ToString();
                array[i, 2] = ThisSMS["status"].ToString();
                array[i, 3] = ThisSMS["cost"].ToString();
                array[i, 4] = ThisSMS["text"].ToString();

            }
            MessageBox.Show(array.ToString());


               // MessageBox.Show((i + 1).ToString() + " +" + HistoryJSON["number"] + " \"" + HistoryJSON["text"] + "\"\n\n");
            

          
            return array;
        }

        public DataTable DataGridInput()
        {
            string[,] StringArray = GetSMSInfoAPI();

            var rows = StringArray.GetLength(0);
            var columns = StringArray.GetLength(1);

            var t = new DataTable();
            for (var c = 0; c < columns; c++)
                t.Columns.Add(new DataColumn(c.ToString()));

            for (var r = 0; r < rows; r++)
            {
                var newRow = t.NewRow();
                for (var c = 0; c < columns; c++)
                    newRow[c] = StringArray[r, c];
                t.Rows.Add(newRow);
            }

            return t;
        }

    }
}
