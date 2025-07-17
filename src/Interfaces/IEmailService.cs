using System;
using Contact.API.Models;

namespace Contact.API.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest request, string templateContent, CancellationToken cancellationToken);
    Task<string> GetEmailTemplateAsync(string templateName, CancellationToken cancellationToken);

}
