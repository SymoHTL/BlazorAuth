using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Model.Util;

namespace Auth.Services;

public class SmtpEmailSender(
    IOptions<MailSettings> optionsAccessor,
    ILogger<SmtpEmailSender> logger)
    : IEmailSender {
    private readonly NetworkCredential _credentials =
        new(optionsAccessor.Value.Username, optionsAccessor.Value.Password);

    private readonly string _from = optionsAccessor.Value.From;
    private readonly string _host = optionsAccessor.Value.Host;
    private readonly int _port = optionsAccessor.Value.Port;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
        try {
            using var smtpClient = SmtpFactory();
            using var mail = MailFactory(email, subject, htmlMessage);
            await smtpClient.SendMailAsync(mail);
            logger.LogInformation("Email sent");
        }
        catch (Exception e) {
            logger.LogError(e, "Error sending email");
        }
    }


    private SmtpClient SmtpFactory() {
        var smtpClient = new SmtpClient();
        smtpClient.Host = _host;
        smtpClient.Port = _port;
        smtpClient.Credentials = _credentials;
        smtpClient.EnableSsl = true;
        return smtpClient;
    }

    private MailMessage MailFactory(string to, string subject, string body) {
        var mail = new MailMessage {
            From = new MailAddress(_from),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(to.Trim());
        return mail;
    }
}