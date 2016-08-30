using System;
using System.Net.Mail;

namespace ConceptONE.Infrastructure.Net
{
    public class Mail
    {
        public void SendEmail(string subject, string toAddress, string fromAddress, string body)
        {
            MailMessage mail = new MailMessage();

            mail.From = new System.Net.Mail.MailAddress(fromAddress);
            mail.To.Add(toAddress);
            mail.Subject = string.Format(subject);
            mail.IsBodyHtml = true;
            mail.Body = GetFormattedBody(body);

            SmtpClient smtp = new SmtpClient();
            Logger.LogActivity("Senging email [Subject: {0}]", subject);

            smtp.Send(mail);
        }

        private string GetFormattedBody(string body)
        {
            const string BODY_FORMAT = "<font face='Courier New' color=#000000>{0}</font>";
            string result = string.Format(BODY_FORMAT, body);

            return result;
        }
    }
}
