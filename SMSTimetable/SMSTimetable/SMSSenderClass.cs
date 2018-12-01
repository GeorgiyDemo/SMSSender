using System.Net;
using System.IO;
using System.Windows;
namespace SMSTimetable
{
    public class SMSSenderClass
    {
        const string From = "SMS Aero";

        public string sms_send(Request request)
        {
            string Data = "";


            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.text != null)
            {
                Data += "text=" + request.text + "&";
            }
            if (request.channel != null)
            {
                Data += "channel=" + request.channel + "&";
            }
            if (request.dateSend != null)
            {
                Data += "dateSend=" + request.dateSend + "&";
            }
            if (request.callbackUrl != null)
            {
                Data += "callbackUrl=" + request.callbackUrl + "&";
            }
            Data += "sign=" + From;
            string method = "sms/send/";
            return send(Data, method);
        }
        public string check_send(Request request)
        {
            string Data = "id=" + request.id;
            string method = "sms/status/";
            return send(Data, method);
        }
        public string sms_list(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.text != null)
            {
                Data += "text=" + request.text + "&";
            }
            if (request.page != null)
            {
                Data += "page=" + request.page + "&";
            }
            string method = "sms/list/";
            return send(Data, method);
        }
        public string balance()
        {
            string Data = "";
            string method = "balance";

            return send(Data, method);
        }
        private string send(string Data, string method)
        {

            string basic_auth_data = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(JustTokenClass.SMS_Login + ":" + JustTokenClass.SMS_APIKey));

            string url = ("http://gate.smsaero.ru/v2/" + method);

            WebRequest req = WebRequest.Create(url + "?" + Data);
            req.Headers.Add("Authorization", "Basic " + basic_auth_data);
            string Out;
            try
            {
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                Out = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Net.WebException)
            {
                MessageBox.Show("Ошибка отправки сообщения через смс, возможно не хватает денег");
                Out = "false";
            }

            return Out;

        }

    }

    public class Request
    {
        public int id { get; set; }
        public string number { get; set; }
        public string[] numbers { get; set; }
        public string text { get; set; }
        public string channel { get; set; }
        public int dateSend { get; set; }
        public string callbackUrl { get; set; }
        public int page { get; set; }
        public string name { get; set; }
        public dynamic groupId { get; set; }
        public int birthday { get; set; }
        public string sex { get; set; }
        public string lname { get; set; }
        public string fname { get; set; }
        public string sname { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string operat { get; set; }
        public string sign { get; set; }
        public string imageSource { get; set; }
        public string textButton { get; set; }
        public string linkButton { get; set; }
        public string signSms { get; set; }
        public string channelSms { get; set; }
        public string textSms { get; set; }
        public string priceSms { get; set; }
        public int sendingId { get; set; }
        public int sum { get; set; }
        public int cardId { get; set; }
    }
}

