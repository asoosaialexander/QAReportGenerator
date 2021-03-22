using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace QAReportGenerator
{
    public static class OutlookClient
    {
        public static void SendMail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Soosai Alexander", "soosai_antony@hotmail.com"));
            message.To.Add(new MailboxAddress("Soosai Alexander", "asoosaialexander@gmail.com"));
            message.Subject = "Test Email";

            message.Body = new TextPart("plain")
            {
                Text = "Test MailKit email"
            };

            using (var client = new SmtpClient())
            {
                // office 365
                //client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);

                // hotmail
                client.Connect("smtp.live.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("joey", "password");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}