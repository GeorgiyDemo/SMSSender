using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows;

namespace SMSTimetable
{
    class ParseJSONLogicClass
    {
        string json;

        public void GetTimetableJSON()
        {
            var wc = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            json = wc.DownloadString("http://46.101.17.171:500/api/v1/get_json/");

        }

        public string GetTimetableByGroup(string groupstr)
        {
            json = json.Replace("—", "-");
            MessageBox.Show(json);
            string outstr = "Расписание "+groupstr+"\n";
            JToken itemtoken;
            JObject.Parse(json).TryGetValue(groupstr, out itemtoken);
            if (itemtoken == null)
                return "false";
            for (int i = 0; i < 5; i++)
                outstr += (i+1).ToString()+" "+itemtoken[i].ToString()+"\n";
            return outstr;
        }

    }
}
