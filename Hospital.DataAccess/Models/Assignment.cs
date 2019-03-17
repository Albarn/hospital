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

    public class Assignment
    {
        public string AssignmentId { get; set; }

        [Required]
        public string TreatmentId { get; set; }
        public Treatment Treatment { get; set; }

        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public string NurseId { get; set; }
        public Nurse Nurse { get; set; }
        
        public DateTime AssignmentDate { get; set; }
        public DateTime? FinishDate { get; set; }

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
