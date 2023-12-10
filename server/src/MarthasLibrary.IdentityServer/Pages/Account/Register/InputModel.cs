using System.ComponentModel.DataAnnotations;

namespace MarthasLibrary.IdentityServer.Pages.Account.Register;

public class InputModel
{
    public string ReturnUrl { get; set; }

    [Required]
    [MaxLength(250)]
    [Display(Name = "Given name")]
    public string GivenName { get; set; }

    [Required]
    [MaxLength(250)]
    [Display(Name = "Family name")]
    public string FamilyName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}