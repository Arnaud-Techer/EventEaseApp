namespace EventEaseApp.Models;

using System.ComponentModel.DataAnnotations;
using System;

public class AppEvent
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name for the event is required.")]

    [StringLength(50, ErrorMessage = "Maximum lenght for an event name is 50 characters.")]
    public string? EventName { get; set; }

    [StringLength(500, ErrorMessage = "Maximum lenght for the event description is 500 characters.")]
    public string? EventDescription { get; set; }

    [Required(ErrorMessage = "Date for an event is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    [FutureDate(ErrorMessage = "Event date must be today or in the future.")]
    public DateTime EventDate { get; set; }

    [Required(ErrorMessage = "Location for the event is required.")]
    [StringLength(100, ErrorMessage = "Maximum length for the event location is 100 characters.")]
    public string? EventLocation { get; set; }
}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date.Date < DateTime.Now.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be today or in the future.");
            }
        }
        return ValidationResult.Success;
    }
}