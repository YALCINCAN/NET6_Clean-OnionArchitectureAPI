using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Persistence.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }
        public async Task ConfirmationMailAsync(string link, string email)
        {
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL
            };
            var subject = $"www.CleanArchitecture.com || Confirm Email";
            var body = "<h2>Please click this link for confirm email</h2><hr/>";
            body += $"<a href='{link}'>Confirmation Link</a>";
            await client.SendMailAsync(
            new MailMessage(_emailSettings.Email, email, subject, body) { IsBodyHtml = true }
            );
        }

        public async Task ForgetPasswordMailAsync(string link, string email)
        {
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL
            };
            var subject = $"www.CleanArchitecture.com || Reset Password";
            var body = "<h2>Please click this link for reset password</h2><hr/>";
            body += $"<a href='{link}'>reset password link</a>";
            await client.SendMailAsync(
            new MailMessage(_emailSettings.Email, email, subject, body) { IsBodyHtml = true }
            );
        }
        
        public async Task SendNewPasswordAsync(string password, string email)
        {
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL
            };
            var subject = $"www.CleanArchitecture.com || New Password";
            var body = "<h2>Your New Password</h2><hr/>";
            body += $"<p>Password : {password}</p>";
            await client.SendMailAsync(
            new MailMessage(_emailSettings.Email, email, subject, body) { IsBodyHtml = true }
            );
        }
    }
}
