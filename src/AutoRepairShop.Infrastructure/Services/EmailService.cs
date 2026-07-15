using AutoRepairShop.Application.Interfaces.Messages;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AutoRepairShop.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> options,
    ILogger<EmailService> logger
) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendAsync(IEmailMessage message)
    {
        ValidateSettings();

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart("plain") { Text = message.Body };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.SmtpHost,
            _settings.SmtpPort,
            GetSecureSocketOptions()
        );

        if (!string.IsNullOrWhiteSpace(_settings.UserName))
        {
            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
        }

        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);

        logger.LogInformation(
            "Email sent to {Recipient} with subject {Subject}",
            message.To,
            message.Subject
        );
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
            throw new InvalidOperationException("Email:SmtpHost is required.");

        if (string.IsNullOrWhiteSpace(_settings.FromAddress))
            throw new InvalidOperationException("Email:FromAddress is required.");
    }

    private SecureSocketOptions GetSecureSocketOptions()
    {
        if (!_settings.UseSsl)
            return SecureSocketOptions.None;

        return _settings.SmtpPort switch
        {
            465 => SecureSocketOptions.SslOnConnect,
            _ => SecureSocketOptions.StartTlsWhenAvailable,
        };
    }
}
