#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CS8602
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models;

public class User 
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "First Name must be a minimum of 2 characters")]
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Last Name must be a minimum of 2 characters")]
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [UniqueEmail]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be a minimum of 8 characters")]
    public string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [DisplayName("Confirm Password")]
    public string ConfirmPassword { get; set; }
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value == null)
        {
            return new ValidationResult("Email is required!");
        }
    
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if(_context.Users.Any(e => e.Email == value.ToString()))
        {
            return new ValidationResult("Email in use. Please use a different email or log in");
        } else {
            return ValidationResult.Success;
        }
    }
}

