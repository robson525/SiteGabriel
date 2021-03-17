using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Rifa.Message;
using Rifa.Models;

namespace Rifa
{
    public class Mail
    {
        #region Singleton

        private static Mail _instance;

        public static Mail Instance => _instance ?? (_instance = new Mail());

        private Mail() { }
        #endregion

        public const string From = "charifagabriel@gmail.com";
        public const string Password = "77TQ3bLr26xbJs4";


        public async Task<bool> Reserved(RifaItem item)
        {
#if DEBUG
            return true;
#endif

            return await TaskEx.Run(() =>
            {
                string message = Resource.Reserved
                    .Replace("_NAME_", item.Name)
                    .Replace("_NUMBER_", item.Id.ToString())
                    .Replace("_MESSAGE_", this.ReplaceBreakLine(item.Comment));
                return SendEmail(message, item);
            });
        }

        private bool SendEmail(string msg, RifaItem item)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    EnableSsl = true,
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(From, Password)
                };

                MailAddress from = new MailAddress(From, "Sandro & Traís", System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(item.Email);

                MailMessage message = new MailMessage(from, to)
                {
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    Body = msg,
                    Subject = $"Chá Rifa do Gabriel - Nº {item.Id}",
                    IsBodyHtml = true
                };

                client.Send(message);
                message.Dispose();
                Logger.Instance.WriteInfo($"Email set to: {item}");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError($"Sending email to: {item}", ex);
                return false;
            }
        }

        private string ReplaceBreakLine(string msg)
        {
            return msg
                .Replace(Environment.NewLine, "<br />")
                .Replace("\r", "<br />")
                .Replace("\n", "<br />");
        } 
    }
}
