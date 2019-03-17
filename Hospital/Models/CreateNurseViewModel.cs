using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class CreateNurseViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 8)]
        [RegularExpression("^([a-z|A-Z|']+)( ([a-z|A-Z|']+))*$", ErrorMessage = "Name is not correct.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}