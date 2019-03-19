﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.Models
{
    public class Treatment
    {
        public string TreatmentId { get; set; }

        [Required]
        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        [Required]
        public string PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// complaint that patient came to doctor with
        /// </summary>
        [Required]
        public string Complaint { get; set; }

        public DateTime? FinishDate { get; set; }
        public string FinalDiagnosis { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public Treatment()
        {
            TreatmentId = Guid.NewGuid().ToString();
            Assignments = new List<Assignment>();
        }
    }
}
