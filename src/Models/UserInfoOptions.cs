using System;

namespace Contact.API.Models;

public class UserInfoOptions
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Github { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Whatsapp { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
