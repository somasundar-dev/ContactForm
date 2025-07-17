using System;
using System.Text.Json;
using Contact.API.Constants;
using Contact.API.Interfaces;
using Contact.API.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Contact.API.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly UserInfoOptions _profile;
    private readonly SmtpOptions _smtpOptions;

    public EmailService(ILogger<EmailService> logger, IOptions<UserInfoOptions> profile, IOptions<SmtpOptions> smtpOptions)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(profile, nameof(profile));
        ArgumentNullException.ThrowIfNull(smtpOptions, nameof(smtpOptions));

        _logger = logger;
        _profile = profile.Value;
        _smtpOptions = smtpOptions.Value;

        _logger.LogInformation("EmailService initialized with SMTP options: {0}", JsonSerializer.Serialize(smtpOptions.Value));
    }

    public async Task<bool> SendEmailAsync(EmailRequest request, string templateContent, CancellationToken ct)
    {
        _logger.LogInformation("Preparing to send email to {0} from {1}", request.Email, _smtpOptions.Username);

        _logger.LogInformation("Request: {0}", JsonSerializer.Serialize(request));
        ct.ThrowIfCancellationRequested();
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtpOptions.DisplayName, _smtpOptions.Username));
        message.To.Add(new MailboxAddress(request.Name, request.Email));
        message.Bcc.Add(new MailboxAddress(_profile.Name, _profile.Email));
        message.Subject = EmailTemplateConstants.Subject;
        _logger.LogInformation("Email subject set to: {0}", EmailTemplateConstants.Subject);

        var bodyBuilder = new BodyBuilder { HtmlBody = templateContent };
        message.Body = bodyBuilder.ToMessageBody();

        _logger.LogInformation("Connecting to SMTP server...");
        ct.ThrowIfCancellationRequested();
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.StartTls);
        _logger.LogInformation("Connected to SMTP server.");

        _logger.LogInformation("Authenticating SMTP client...");
        ct.ThrowIfCancellationRequested();
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        _logger.LogInformation("SMTP client authenticated.");

        _logger.LogInformation("Sending email...");
        ct.ThrowIfCancellationRequested();
        await client.SendAsync(message);
        _logger.LogInformation("Email sent successfully.");

        client.Disconnect(true);
        _logger.LogInformation("Disconnected from SMTP server.");
        return true;
    }

    public async Task<string> GetEmailTemplateAsync(string templateName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateName);
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }
        ct.ThrowIfCancellationRequested();
        _logger.LogInformation("Reading template file: {0}", templatePath);
        using var reader = new StreamReader(templatePath);
        var content = await reader.ReadToEndAsync(ct);
        _logger.LogInformation("Template file read successfully: {0}", templatePath);
        ct.ThrowIfCancellationRequested();
        content = ReplacePlaceholders(content);
        _logger.LogInformation("Template file processed successfully: {0}", templatePath);
        _logger.LogInformation("Processed content: {0}", content);
        return content;
    }

    private string ReplacePlaceholders(string template)
    {
        _logger.LogInformation("Replacing placeholders in template.");
        _logger.LogInformation("Template: {0}", template);
        _logger.LogInformation("Profile: {0}", JsonSerializer.Serialize(_profile));

        return template
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Name, _profile.Name)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Email, _profile.Email)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Contact, _profile.Contact)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Website, _profile.Website)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Github, _profile.Github)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.LinkedIn, _profile.LinkedIn)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Whatsapp, _profile.Whatsapp)
            .Replace(EmailTemplateConstants.UserInfoPlaceholder.Address, _profile.Address);
    }
}
