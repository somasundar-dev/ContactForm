using System.ComponentModel.DataAnnotations;
using Contact.API.Constants;
using Contact.API.Interfaces;
using Contact.API.Models;
using Contact.API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
builder.Services.Configure<UserInfoOptions>(configuration.GetSection("UserInfo"));
builder.Services.AddCors(policy =>
{
    policy.AddPolicy(
        "Production",
        builder =>
        {
            Console.WriteLine("Configuring CORS policy for production environment...");
            var allowedOrigins = configuration["AllowedOrigins"];
            if (string.IsNullOrWhiteSpace(allowedOrigins))
            {
                ArgumentNullException.ThrowIfNull(allowedOrigins, nameof(allowedOrigins));
            }

            builder.WithOrigins(allowedOrigins.Split(",")).AllowAnyMethod().AllowAnyHeader();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("Production");

app.MapGet(
    "/api/contact",
    () =>
    {
        return Results.Ok("Email Service API is Running");
    }
);

app.MapPost(
    "/api/contact/send-email",
    async (EmailRequest emailRequest, CancellationToken cancellationToken, IEmailService emailService) =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(emailRequest, new ValidationContext(emailRequest), validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }

        var emailContent = await emailService.GetEmailTemplateAsync(
            EmailTemplateConstants.Template1,
            cancellationToken
        );

        emailContent = emailContent
            .Replace(EmailTemplateConstants.SubmitterPlaceholder.Name, emailRequest.Name)
            .Replace(EmailTemplateConstants.SubmitterPlaceholder.Email, emailRequest.Email)
            .Replace(EmailTemplateConstants.SubmitterPlaceholder.Message, emailRequest.Message);

        var emailSent = await emailService.SendEmailAsync(emailRequest, emailContent, cancellationToken);
        if (!emailSent)
        {
            return Results.BadRequest("Failed to send email.");
        }
        return Results.Ok("Email sent successfully.");
    }
);

app.Run();
