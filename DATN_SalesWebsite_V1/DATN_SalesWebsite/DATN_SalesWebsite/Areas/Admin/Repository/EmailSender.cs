using System.Net.Mail;
using System.Net;

namespace DATN_SalesWebsite.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nhoxlongbi123@gmail.com", "kyxo bagj bgxk yahj")
            };

            return client.SendMailAsync(
                new MailMessage(from: "nhoxlongbi123@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
