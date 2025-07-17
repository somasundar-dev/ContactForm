using System;

namespace Contact.API.Constants;

public static class EmailTemplateConstants
{
    public const string Template1 = "Templates/Template1.html";

    public const string Subject = "Thank you for reaching out!";

    public static class SubmitterPlaceholder
    {
        public const string Name = "#SUBMITTER_NAME#";
        public const string Email = "#SUBMITTER_EMAIL#";
        public const string Message = "#SUBMITTER_MESSAGE#";
    }

    public static class UserInfoPlaceholder
    {
        public const string Name = "#NAME#";
        public const string Email = "#EMAIL#";
        public const string Contact = "#CONTACT#";
        public const string Website = "#WEBSITE#";
        public const string Github = "#GITHUB#";
        public const string LinkedIn = "#LINKEDIN#";
        public const string Whatsapp = "#WHATSAPP#";
        public const string Address = "#ADDRESS#";
    }

}
