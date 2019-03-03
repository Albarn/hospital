using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string FullName { get; set; }
        [Required]
        public Position Position { get; set; }
    }
}
