using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class TreatmentItemViewModel
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string TreatmentId { get; set; }
        public string Diagnosis { get; set; }
        public DateTime StartDate { get; set; }
    }
}