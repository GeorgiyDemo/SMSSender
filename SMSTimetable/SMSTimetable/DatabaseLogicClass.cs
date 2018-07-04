using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace SMSTimetable
{
    public class DatabaseLogicClass
    {
        public static string GetSQL(string sql)
        {
            // строка подключения к БД
            string connStr = "ЛА ЛА ЛА ПАРАМЕТРЫ КОННЕКТА";
            // создаём объект для подключения к БД
            MySqlConnection conn = new MySqlConnection(connStr);
            // устанавливаем соединение с БД
            conn.Open();
            // запрос
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(sql, conn);
            // выполняем запрос и получаем ответ
            string name = command.ExecuteScalar().ToString();
            // выводим ответ в консоль
            // закрываем соединение с БД
            conn.Close();
            return name;
        }
    }
}
