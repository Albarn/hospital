using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.Models
{
    public enum AssignmentType
    {
        Procedure,
        Medicine,
        Operation
    }

    /// <summary>
    /// assignment, that doctor gives to patient
    /// </summary>
    public class Assignment
    {
        public string AssignmentId { get; set; }

        [Required]
        public string TreatmentId { get; set; }
        public Treatment Treatment { get; set; }

        //assignment can be done by doctor or nurse
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public string NurseId { get; set; }
        public Nurse Nurse { get; set; }
        
        /// <summary>
        /// date that assignment must be done
        /// </summary>
        [Display(Name ="Assignment Date")]
        [DataType(DataType.Date)]
        public DateTime AssignmentDate { get; set; }

        /// <summary>
        /// actual finish date
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// reason for assignment
        /// </summary>
        [Required]
        public string Diagnosis { get; set; }
        [Required]
        public string Instruction { get; set; }
        public AssignmentType Type { get; set; }

        public Assignment()
        {
            AssignmentId = Guid.NewGuid().ToString();
        }
    }
}
