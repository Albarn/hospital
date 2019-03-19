using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

namespace Hospital.DataAccess.Models
{
    /// <summary>
    /// User Role
    /// </summary>
    public enum Role
    {
        Admin =     0b0000_0001,
        Doctor =    0b0000_0010,
        Nurse =     0b0000_0100,
        Patient =   0b0000_1000
    }

    public class User : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        // is 2 step registration completed:
        // Admin created user,
        // then attached doctor|nurse|patient to account
        public bool IsConfirmed { get; set; }
        
        /// <summary>
        /// mask of user roles
        /// </summary>
        public int Roles { get; set; }

        #region helpers to work with Roles

        /// <summary>
        /// check that Roles mask has role bit
        /// </summary>
        public bool IsInRole(Role role)
        {
            return (Roles & (int)role) != 0;
        }

        /// <summary>
        /// add role bit to Roles
        /// </summary>
        public void AddRole(Role role)
        {
            Roles |= (int)role;
        }

        /// <summary>
        /// remove role bit from Roles
        /// </summary>
        public void RemoveRole(Role role)
        {
            Roles &= ~(int)role;
        }

        /// <summary>
        /// get string list representation of Roles
        /// </summary>
        public List<string> GetRoles()
        {
            var ans = new List<string>();
            foreach(var role in (Role[])Enum.GetValues(typeof(Role)))
            {
                if (IsInRole(role))
                {
                    ans.Add(role.ToString());
                }
            }
            return ans;
        }

        #endregion

        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public virtual Doctor Doctor { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
