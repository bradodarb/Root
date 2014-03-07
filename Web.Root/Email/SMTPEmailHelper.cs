using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Web.Root.Email
{
    public static class SMTPEmailHelper
    {
        public static void SendEmail(string subject, string body, List<string> to, string from, List<string> cc = null, List<string> bcc = null, bool isHtml = true, MailPriority priority = MailPriority.Normal)
        {

            MailMessage message = new MailMessage();
            message.BodyEncoding = System.Text.ASCIIEncoding.UTF8;
            message.IsBodyHtml = isHtml;
            message.Priority = priority;

            message.From = new MailAddress(from);

            //Add list of TO recipients
            if (to != null) to.ForEach(item => message.To.Add(new MailAddress(item.ToString())));

            //Add list of CC recipients
            if (cc != null) cc.ForEach(item => message.CC.Add(new MailAddress(item.ToString())));

            //Add list of BCC recipients
            if (bcc != null) bcc.ForEach(item => message.Bcc.Add(new MailAddress(item.ToString())));

            message.Subject = subject;
            message.Body = body;

            SmtpClient client = new SmtpClient();
            client.Send(message);

        }
        public static void SendEmail(string subject, string body, string to, string from, bool isHtml = true, MailPriority priority = MailPriority.Normal)
        {

            MailMessage message = new MailMessage();
            message.BodyEncoding = System.Text.ASCIIEncoding.UTF8;

            message.IsBodyHtml = isHtml;
            message.Priority = priority;
            message.From = new MailAddress(from);
            foreach (string s in to.Split(';'))
            {
                message.To.Add(new MailAddress(s));
            }
  
            message.Subject = subject;
            message.Body = body;

            SmtpClient client = new SmtpClient();
            client.Send(message);

        }
    }
}
