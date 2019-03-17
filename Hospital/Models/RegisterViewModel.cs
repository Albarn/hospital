using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9_@\.]+$", ErrorMessage = "Only alphabetical letters, digits and _, @, . characters allowed.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20,ErrorMessage = "Password must contain from 6 to 20 characters.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}