using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace QAReportGenerator
{
    public static class EmailClient
    {
        private static IConfigurationRoot config;
        static EmailClient()
        {
            config = Program.configurationRoot;
        }
        public static void SendMail(string attachmentFileName)
        {
            var fromAddress = new MailAddress(config["Email:FromAddress"], config["Email:FromName"]);
            var toAddress = new MailAddress(config["Email:ToAddress"], config["Email:ToName"]);
            string fromPassword = config["EmailPassword"];

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            const string subject = "Sonarqube QA Report";
            const string body = "Hi, The latest QA report fetch from sonarqube is attached for your review. Thanks!";
            var attachment = new Attachment(
                File.Open(attachmentFileName, FileMode.Open), attachmentFileName);
            attachment.ContentType = new ContentType("application/vnd.ms-excel");

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                message.Attachments.Add(attachment);
                smtp.Send(message);
            }

            if (File.Exists(attachmentFileName))
            {
                File.Delete(attachmentFileName);
            }
        }
    }
}