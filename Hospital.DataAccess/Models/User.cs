using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.Models
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // is 2 step registration completed:
        // admin created user,
        // then attached doctor|nurse|patient to account
        public bool IsConfirmed { get; set; }

        private string rolesString;
        public string RolesString
        {
            get => Roles.Count == 0 ? "" : Roles.Aggregate((r1, r2) => r1 + ", " + r2);
            set
            {
                rolesString = value;
                Roles = rolesString.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
        [NotMapped]
        public List<string> Roles { get; private set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            RolesString = "";
        }

        public virtual Doctor Doctor { get; set; }
    }
}
