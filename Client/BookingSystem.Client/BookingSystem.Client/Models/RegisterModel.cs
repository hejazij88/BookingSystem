using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Client.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "نام الزامی است.")]
    [MaxLength(100, ErrorMessage = "نام نباید بیشتر از 100 کاراکتر باشد.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
    [MaxLength(100, ErrorMessage = "نام خانوادگی نباید بیشتر از 100 کاراکتر باشد.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ایمیل الزامی است.")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور الزامی است.")]
    [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
    [Compare(nameof(Password), ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

