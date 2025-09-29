using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Models;

public class EventRegistrationModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(255, ErrorMessage = "Email address cannot exceed 255 characters.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500, ErrorMessage = "Special requirements cannot exceed 500 characters.")]
    [Display(Name = "Special Requirements")]
    public string? SpecialRequirements { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions to register.")]
    [Display(Name = "Accept Terms")]
    public bool AcceptTerms { get; set; } = false;
}

