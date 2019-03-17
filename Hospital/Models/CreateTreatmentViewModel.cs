using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class CreateTreatmentViewModel
    {
        [Required]
        public string DoctorFullName { get; set; }
        [Required]
        [StringLength(30,MinimumLength = 5)]
        public string Complaint { get; set; }
    }
}