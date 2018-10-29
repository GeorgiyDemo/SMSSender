using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

namespace SMSTimetable
{
    class SMSStatusClass
    {

        public string[,] GetSMSInfoAPI()
        {

            var StatusDictionary = new Dictionary<string, string>
            {
                {"0", "в очереди"},
                {"1", "доставлено"},
                {"2", "не доставлено"},
                {"3", "передано"},
                {"4", "ожидание статуса сообщения"},
                {"6", "сообщение отклонено"},
                {"8", "на модерации"}
            };

            SMSSenderClass SMSHistory_obj = new SMSSenderClass();
            dynamic HistoryJSON = JObject.Parse(SMSHistory_obj.sms_list(new Request { page = 1 }));
            string[,] array = new string[10, 5];
            for (int i = 0; i < 10; i++)
            {
                JObject ThisSMS = HistoryJSON.data[i.ToString()];
                array[i, 0] = (i + 1).ToString();
                array[i, 1] = ThisSMS["number"].ToString();
                array[i, 2] = StatusDictionary[ThisSMS["status"].ToString()];
                array[i, 3] = ThisSMS["cost"].ToString()+" руб.";
                string[] thisarr = ThisSMS["text"].ToString().Replace("\n", " ").Split(' ');
                array[i, 4] = (thisarr[0] == "Расписание") ? thisarr[0] + " " + thisarr[1] : ThisSMS["text"].ToString().Replace("\n", " ");

            }

            return array;
        }

        public DataTable DataGridInput()
        {
            string[,] StringArray = GetSMSInfoAPI();
            string[] FormatArray = { "№", "Телефон", "Статус", "Стоимость", "Текст" };
            var rows = StringArray.GetLength(0);
            var columns = StringArray.GetLength(1);

            var t = new DataTable();
            for (var c = 0; c < columns; c++)
                t.Columns.Add(new DataColumn(FormatArray[c]));

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
