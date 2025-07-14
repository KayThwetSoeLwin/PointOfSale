using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PointOfSale.Shared.ViewModels
{
    public class StaffResetPasswordViewModel
    {
        public int StaffId { get; set; }

        [BindNever]  // Prevent model binding and validation
        public string FullName { get; set; }

        [BindNever]  // Prevent model binding and validation
        public string Username { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
