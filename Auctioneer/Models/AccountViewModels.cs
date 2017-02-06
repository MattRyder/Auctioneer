using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Auctioneer.Models
{
    /// <summary>
    /// View Model for the Registration Form
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password Confirmation")]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string PasswordConfirmation { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// View Model for the Login Form
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Keep me signed in")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// View Model for showing the list of available oAuth services
    /// </summary>
    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }
}