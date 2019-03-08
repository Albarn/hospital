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
        [Index(IsUnique = true, Order = 1)]
        public string UserId { get; set; }
        public User User { get; set; }

        [StringLength(30)]
        [Required]
        [Index(IsUnique = true, Order = 2)]
        public string FullName { get; set; }
        [Required]
        public Position Position { get; set; }
    }
}
