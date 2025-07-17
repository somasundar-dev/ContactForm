using System;
using Contact.API.Models;

namespace Contact.API.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string email, string name, string message, CancellationToken cancellationToken);
    Task<string> GetEmailTemplateAsync(string templateName, CancellationToken cancellationToken);

}
