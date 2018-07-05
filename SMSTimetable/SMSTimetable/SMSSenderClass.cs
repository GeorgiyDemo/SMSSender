using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;
namespace SMSTimetable
{
    public class SMSSenderClass
    {
  
        const string From = "SMS Aero";


        public static void SMSWorker()
        {
            //ВСЕ, ЧТО ЗДЕСЬ НАПИСАНО НАДО РЕАЛИЗОВЫВАТЬ НА СТОРОНЕ ТОЙ ЧАСТИ КОДА, ГДЕ НУЖНО РАБОТАТЬ С СМС ПУТЕМ СОЗДАНИЯ ОБЪЕКТА


            // Отправка SMS сообщений
            //string[] numbers = new string[] { "79994935590" };
            //var request = new Request { numbers = numbers, text = "Привет!", channel = "DIRECT" };
            //smsc.sms_send(request);

            // Проверка статуса SMS сообщения 
            //var request1 = new Request { id = 44197982 };
            //smsc.check_send(request1);

            // Получение списка отправленных сообщений
            // var request = new Request { text = "Hello"};
            // smsc.sms_list(request);

            // Запрос баланса
            // smsc.balance();

            // Добавление группы
            // var request = new Request { name = "Fish"};
            // smsc.group_add(request);

            // Удаление группы
            // var request = new Request { id = 231697 };
            // smsc.group_delete(request);

            // Получение списка групп
            // var request = new Request { };
            // smsc.group_list(request);

            // Добавление контакта
            //var request1 = new Request { number = "+79997335592", fname = "КАРТИНА" , sex = "female"};
            //smsc.contact_add(request1); 

            // Удаление контакта
            // var request = new Request { id = 123};
            // smsc.contact_delete(request);

            // Список контактов
            // var request = new Request { number = "71234567890" };
            // smsc.contact_list(request);

            // Создание запроса на проверку HLR
            // string[] numbers = new string[] { "71234567890", "71234567891" };
            // var request = new Request { numbers = numbers };
            // smsc.hlr_check(request);

            // Определение оператора
            // var request = new Request { number = "791234567890" };
            // smsc.number_operator(request);

        }

        /**
        * Отправка сообщения
        * @param numbers dynamic    - Номер телефона
        * @param text string        - Текст сообщения
        * @param channel string     - Канал отправки
        * @param dateSend integer   - Дата для отложенной отправки сообщения (в формате unixtime)
        * @param callbackUrl string - url для отправки статуса сообщения в формате http://mysite.com или https://mysite.com (в ответ система ждет статус 200)
        * @return string(json)
        */
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

        /**
         * Проверка статуса SMS сообщения
         * @param   id int - Идентификатор сообщения
         * @return  string(json)
         */
        public string check_send(Request request)
        {
            string Data = "id=" + request.id;
            string method = "sms/status/";
            return send(Data, method);
        }

        /**
         * Получение списка отправленных sms сообщений
         * @param   number      string  - Фильтровать сообщения по номеру телефона
         * @param   text        string  - Фильтровать сообщения по тексту
         * @param   page        integer - Номер страницы
         * @return  string(json)
         */
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

        /**
         * Запрос баланса
         * @return string(json)
         */
        public string balance()
        {
            string Data = "";
            string method = "balance";

            return send(Data, method);
        }

        /**
        * Добавление группы
        * @param name string - Имя  группы
        * @return string(json)
        */
        public string group_add(Request request)
        {
            string Data = "name=" + request.name;
            string method = "group/add/";

            return send(Data, method);
        }

        /**
        * Удаление группы
        * @param id int - Идентификатор группы
        * @return string(json)
        */
        public string group_delete(Request request)
        {
            string Data = "id=" + request.id;
            string method = "group/delete/";

            return send(Data, method);
        }

        /**
         * Получение списка групп
         * @param page int - Пагинация
         * @return string(json)
         */
        public string group_list(Request request)
        {
            string Data = "";
            if (request.page != null)
            {
                Data += "page=" + request.page;
            }
            string method = "group/list/";

            return send(Data, method);
        }

        /**
        * Добавление контакта
        * @param number     string  - Номер абонента
        * @param groupId    int     - Идентификатор группы
        * @param birthday   int     - Дата рождения абонента (в формате unixtime)
        * @param sex        string  - Пол
        * @param lname      string  - Фамилия абонента
        * @param fname      string  - Имя абонента
        * @param sname      string  - Отчество абонента
        * @param param1     string  - Свободный параметр
        * @param param2     string  - Свободный параметр
        * @param param3     string  - Свободный параметр
        * @return string(json)
        */
        public string contact_add(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.groupId != null)
            {
                Data += "groupId=" + request.groupId + "&";
            }
            if (request.birthday != null)
            {
                Data += "birthday=" + request.birthday + "&";
            }
            if (request.sex != null)
            {
                Data += "sex=" + request.sex + "&";
            }
            if (request.lname != null)
            {
                Data += "lname=" + request.lname + "&";
            }
            if (request.fname != null)
            {
                Data += "fname=" + request.fname + "&";
            }
            if (request.sname != null)
            {
                Data += "sname=" + request.sname + "&";
            }
            if (request.param1 != null)
            {
                Data += "param1=" + request.param1 + "&";
            }
            if (request.param2 != null)
            {
                Data += "param2=" + request.param2 + "&";
            }
            if (request.param3 != null)
            {
                Data += "param3=" + request.param3 + "&";
            }
            string method = "contact/add/";
            return send(Data, method);
        }

        /**
        * Удаление контакта
        * @param id int - Идентификатор абонента
        * @return string(json)
        */
        public string contact_delete(Request request)
        {
            string Data = "id=" + request.id;
            string method = "contact/delete/";

            return send(Data, method);
        }

        /**
         * Список контактов
         * @param number    string  - Номер абонента
         * @param groupId   int     - Идентификатор группы
         * @param birthday  int     - Дата рождения абонента (в формате unixtime)
         * @param sex       string  - Пол
         * @param operat    string  - Оператор
         * @param lname     string  - Фамилия абонента
         * @param fname     string  - Имя абонента
         * @param sname     string  - Отчество абонента
         * @param param1    string  - Свободный параметр
         * @param param2    string  - Свободный параметр
         * @param param3    string  - Свободный параметр
         * @param page      integer - Пагинация
         * @return string(json)
         */
        public string contact_list(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number + "&";
            }
            if (request.groupId != null)
            {
                Data += "groupId=" + request.groupId + "&";
            }
            if (request.birthday != null)
            {
                Data += "birthday=" + request.birthday + "&";
            }
            if (request.sex != null)
            {
                Data += "sex=" + request.sex + "&";
            }
            if (request.operat != null)
            {
                Data += "operator=" + request.operat + "&";
            }
            if (request.lname != null)
            {
                Data += "lname=" + request.lname + "&";
            }
            if (request.fname != null)
            {
                Data += "fname=" + request.fname + "&";
            }
            if (request.sname != null)
            {
                Data += "sname=" + request.sname + "&";
            }
            if (request.param1 != null)
            {
                Data += "param1=" + request.param1 + "&";
            }
            if (request.param2 != null)
            {
                Data += "param2=" + request.param2 + "&";
            }
            if (request.param3 != null)
            {
                Data += "param3=" + request.param3 + "&";
            }
            if (request.page != null)
            {
                Data += "page=" + request.page + "&";
            }
            string method = "contact/list/";
            return send(Data, method);
        }

        /**
         * Создание запроса на проверку HLR
         * @param number    string  - Номер абонента
         * @param numbers   array   - Номера телефонов
         * @return string(json)
         */
        public string hlr_check(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            string method = "hlr/status/";
            return send(Data, method);
        }

        /**
         * Определение оператора
         * @param number    string  - Номер абонента
         * @param numbers   array   - Номера телефонов
         * @return string(json)
         */
        public string number_operator(Request request)
        {
            string Data = "";
            if (request.number != null)
            {
                Data += "number=" + request.number;
            }
            if (request.numbers != null)
            {
                foreach (string number in request.numbers)
                {
                    Data += "numbers[]=" + number + "&";
                }
            }
            string method = "number/operator/";
            return send(Data, method);
        }
      
        private string send(string Data, string method)
        {

            string basic_auth_data = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(JustTokenClass.SMS_Login + ":" + JustTokenClass.SMS_APIKey));

            string url = ("http://gate.smsaero.ru/v2/" + method);

            WebRequest req = WebRequest.Create(url + "?" + Data);
            req.Headers.Add("Authorization", "Basic " + basic_auth_data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();

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
        public string operat { get; set; } // оператор
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

