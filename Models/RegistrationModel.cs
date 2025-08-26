namespace EventEaseApp.Models;
using System.ComponentModel.DataAnnotations;

public sealed class RegistrationModel
{
    [Required(ErrorMessage = "Username is required.")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

}   