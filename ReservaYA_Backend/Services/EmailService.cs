using Microsoft.Extensions.Options;


using ReservaYA_Backend.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace ReservaYA_Backend.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string emailTo, string subject, string message)
        {
            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            //    Subject = subject,
            //    Body = message,
            //    IsBodyHtml = true
            //};

            //mailMessage.To.Add(email);

            //using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            //{
            //    client.UseDefaultCredentials = false;
            //    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            //    client.EnableSsl = true;

            //    await client.SendMailAsync(mailMessage);
            //}

            //SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            //client.Port = 587;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //System.Net.NetworkCredential credentials =
            //            new System.Net.NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            //client.EnableSsl = true;
            //client.Credentials = credentials;

            //MailMessage mailmessage = new MailMessage(_emailSettings.SenderEmail, email);
            //mailmessage.Subject = subject;
            //mailmessage.Body = message;
            //client.Send(mailmessage);


            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Username, _emailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
     

        }
    }
}
