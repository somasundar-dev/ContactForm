using System.ComponentModel.DataAnnotations;
using Contact.API.Constants;
using Contact.API.Interfaces;
using Contact.API.Models;
using Contact.API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
builder.Services.Configure<UserInfoOptions>(configuration.GetSection("UserInfo"));
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("Production", builder =>
    {
        var allowedHosts = configuration["AllowedHosts"];
        if (string.IsNullOrWhiteSpace(allowedHosts))
        {
            ArgumentNullException.ThrowIfNull(allowedHosts, nameof(allowedHosts));
        }

        builder.WithOrigins(allowedHosts.Split(","))
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("Production");

app.MapGet("/", () =>
{
    return Results.Ok("Email Service API is Running");
});

app.MapPost("/send-email", async (EmailRequest emailRequest, CancellationToken cancellationToken, IEmailService emailService) =>
{
    cancellationToken.ThrowIfCancellationRequested();
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(emailRequest, new ValidationContext(emailRequest), validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }

    var emailTemplate = await emailService.GetEmailTemplateAsync(EmailTemplateConstants.Template1, cancellationToken);

    emailTemplate = emailTemplate
        .Replace(EmailTemplateConstants.SubmitterPlaceholder.Name, emailRequest.Name)
        .Replace(EmailTemplateConstants.SubmitterPlaceholder.Email, emailRequest.Email)
        .Replace(EmailTemplateConstants.SubmitterPlaceholder.Message, emailRequest.Message);

    var emailSent = await emailService.SendEmailAsync(emailRequest.Email, emailRequest.Name, emailTemplate, cancellationToken);
    if (!emailSent)
    {
        return Results.StatusCode(500);
    }
    return Results.Ok("Email sent successfully.");
});

app.Run();
