using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class CreatePatientViewModel
    {
        [Required]
        [StringLength(30, MinimumLength = 8)]
        [RegularExpression("^([a-z|A-Z|']+)( ([a-z|A-Z|']+))*$", ErrorMessage = "Name is not correct.")]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}