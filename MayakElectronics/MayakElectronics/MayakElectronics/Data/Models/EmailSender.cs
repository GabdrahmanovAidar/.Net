using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MayakElectronics.Data.Models
{
    public class EmailSender
    {
        public static void SendEmail(string toMail, string text)
        {
            var subject = "MayakElectroniks";
            var senderEmail = new MailAddress("mayakelectroniks@gmail.com", "Mayak Electroniks");
            var receiverEmail = new MailAddress(toMail, "Receiver");
            var password = "mayakmayak0101";
            var sub = subject;
            var body = $"Thank you for choosing us! {text}";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(mess);
            }
        }
    }
}
