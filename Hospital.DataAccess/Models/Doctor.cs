using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.Models
{
    public enum Position
    {
        Pediatrician,
        Traumatologist,
        Surgeon
    }
    public class Doctor
    {
        [Key]
        public string UserId { get; set; }
        public User User { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        public Position Position { get; set; }
        
        public ICollection<Treatment> Treatments { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
