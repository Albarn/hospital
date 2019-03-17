using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class CreateDoctorViewModel
    {
        [StringLength(30,MinimumLength = 8)]
        [RegularExpression("^([a-z|A-Z|']+)( ([a-z|A-Z|']+))*$",ErrorMessage ="Name is not correct.")]
        [Required]
        public string FullName { get; set; }
        [Required]
        public Position Position { get; set; }
    }
}