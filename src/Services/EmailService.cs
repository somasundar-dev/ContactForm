using System;
using System.Text.Json;
using Contact.API.Constants;
using Contact.API.Interfaces;
using Contact.API.Models;
using Microsoft.Extensions.Options;

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

    public async Task<bool> SendEmailAsync(string email, string name, string message, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // Simulate sending email
        await Task.Delay(1000, ct);
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
