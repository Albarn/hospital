﻿using Hospital.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class CreateAssignmentViewModel
    {
        [Required]
        [Display(Name ="Assigned")]
        public bool IsDoctor { get; set; }
        [Required]
        public string AssignedFullName { get; set; }
        [Required]
        public AssignmentType Type { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime AssignmentDate { get; set; }
        [Required]
        [StringLength(20,MinimumLength =8)]
        public string Diagnosis { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(200,MinimumLength =8)]
        public string Instruction { get; set; }
    }
}