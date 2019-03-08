using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class DoctorItemViewModel
    {
        public string UserId { get; set; }

        [StringLength(30)]
        [Required]
        public string FullName { get; set; }
        [Required]
        public Position Position { get; set; }

        public int PatientsNumber { get; set; }
    }
}