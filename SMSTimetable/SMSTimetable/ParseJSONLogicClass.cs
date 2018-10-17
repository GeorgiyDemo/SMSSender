using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMSTimetable
{
    class ParseJSONLogicClass
    {
        public static string GetTimetableJSON()
        {
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("http://46.101.17.171:500/api/v1/get_json/");
            }

            MessageBox.Show(json);
            return ("MEOW");
        }
    }
}
