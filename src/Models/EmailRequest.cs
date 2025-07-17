using System;
using System.ComponentModel.DataAnnotations;

namespace Contact.API.Models;

public class EmailRequest
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
    public string Name { get; set; } = null!;

    [Required]
    [DataType(DataType.MultilineText)]
    [StringLength(500, ErrorMessage = "Message cannot be longer than 500 characters.")]
    public string Message { get; set; } = null!;
}
