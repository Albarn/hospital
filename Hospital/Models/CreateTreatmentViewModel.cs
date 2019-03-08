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
        [StringLength(20,MinimumLength = 6)]
        public string DoctorFullName { get; set; }
        [Required]
        public string Complaint { get; set; }
    }
}