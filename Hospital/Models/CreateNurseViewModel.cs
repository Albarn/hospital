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
        [StringLength(20)]
        public string FullName { get; set; }
    }
}