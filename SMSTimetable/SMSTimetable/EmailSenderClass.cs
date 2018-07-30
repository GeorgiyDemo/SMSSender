using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Windows;

namespace SMSTimetable
{
    public class EmailSenderClass
    {
        public static async Task SendEmailAsync(string EmailContent, string EmailReceiver)
        {
            MailAddress from = new MailAddress(JustTokenClass.Email_Login, "DEMKA");
            MailAddress to = new MailAddress(EmailReceiver);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Подтверждение e-mail";
            m.Body = EmailContent;
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential(JustTokenClass.Email_Login, JustTokenClass.Email_Password);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }
    }
}
