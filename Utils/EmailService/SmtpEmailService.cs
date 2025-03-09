using Application.Abstractions.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Utilities.EmailService
{
    public class SmtpEmailService (
        string smptServer,
        int smtPort,
        string senderEmail,
        string senderPassword ,
        SecureSocketOptions secureSocketOptions) 
        : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // create message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderEmail, senderEmail));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            // send email
            using var client = new SmtpClient();
            await client.ConnectAsync(smptServer, smtPort, secureSocketOptions);
            await client.AuthenticateAsync(senderEmail, senderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
