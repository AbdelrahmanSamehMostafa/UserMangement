using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagment.Common.Enum;

namespace UserManagment.Domain.Models;

[Table("User")]
public partial class User : BaseModel
{
    [StringLength(500)]
    [Required(ErrorMessage = "First name is required.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(500)]
    [Required(ErrorMessage = "Last name is required.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces.")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50)]
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500)]
    [Required]
    public string UserName { get; set; } = string.Empty;

    [StringLength(500)]
    public string Password { get; set; } = string.Empty;

    public DateTime? PasswordLastUpdatedDate { get; set; }

    [StringLength(15)]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid mobile number format.")]
    public string? MobileNumber { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(User), nameof(ValidateDateOfBirth))]
    public DateTime DateOfBirth { get; set; }

    [StringLength(500)]
    [Required(ErrorMessage = "Address location is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s,]+$", ErrorMessage = "Address can only contain letters, numbers, spaces, and commas.")]
    public string AddressLocation { get; set; } = string.Empty;

    public Guid? UserImageId { get; set; } // ID of the uploaded image

    // Navigation property to Attachment (Profile Image)
    [ForeignKey("UserImageId")]
    public virtual Attachment ProfileImage { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsLocked { get; set; } = false;

    public UserType UserType { get; set; }

    public int UserTrails { get; set; } = 0;


    // [Required(ErrorMessage = "At least one group must be selected.")]
    // public ICollection<Group> Groups { get; set; } = new List<Group>();

    public static ValidationResult ValidateDateOfBirth(DateTime dateOfBirth, ValidationContext context)
    {
        if (dateOfBirth >= DateTime.UtcNow)
        {
            return new ValidationResult("Date of birth must be less than the current date.");
        }

        return ValidationResult.Success!;
    }
    public static SortExpression<User> SortBy(string sorting)
    {
        // If sorting is null or empty, default to sorting by InsertedDate ascending
        if (string.IsNullOrEmpty(sorting))
        {
            sorting = "default";
        }

        switch (sorting.ToLower())
        {
            case "name_ascending":
                return new SortExpression<User> { Expression = g => g.FirstName, IsDescending = false };

            case "name_descending":
                return new SortExpression<User> { Expression = g => g.FirstName, IsDescending = true };

            case "email_ascending":
                return new SortExpression<User> { Expression = g => g.Email, IsDescending = false };

            case "email_descending":
                return new SortExpression<User> { Expression = g => g.Email, IsDescending = true };

            case "insertedDate_ascending":
                return new SortExpression<User> { Expression = g => g.InsertedDate, IsDescending = false };

            case "insertedDate_descending":
                return new SortExpression<User> { Expression = g => g.InsertedDate, IsDescending = true };

            case "default":
            default:
                return new SortExpression<User> { Expression = g => g.InsertedDate, IsDescending = true };
        }
    }
}

