using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.Models
{
    public class Patient
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime BirthDate { get; set; }
        
        public ICollection<Treatment> Treatments { get; set; }
    }
}
