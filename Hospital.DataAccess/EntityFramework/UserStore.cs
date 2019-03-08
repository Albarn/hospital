using Hospital.DataAccess.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DataAccess.EntityFramework
{
    public class UserStore :
        IUserStore<User>,
        IUserPasswordStore<User>,
        IUserRoleStore<User>,
        IUserLockoutStore<User, string>,
        IUserTwoFactorStore<User, string>
    {
        private HospitalDbContext db = HospitalDbContext.Create();

        //implements interfaces with UserName, Id, Roles and PasswordHash property
        #region IUserStore
        public Task CreateAsync(User user)
        {
            db.Users.Add(user);
            return db.SaveChangesAsync();
        }

        public void Dispose() { }

        public Task<User> FindByIdAsync(string userId)
        {
            return Task.FromResult(db.Users.Find(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return Task.FromResult(db
                .Users
                .Where(u => u.UserName == userName)
                ?.SingleOrDefault());
        }

        public Task UpdateAsync(User user)
        {
            db.Entry(db.Users.Find(user.Id)).CurrentValues.SetValues(user);
            return db.SaveChangesAsync();
        }

        public Task DeleteAsync(User user) => throw new NotImplementedException();
        #endregion

        #region IUserPasswordStore
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.PasswordHash!=null);
        }
        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(User user, string roleName)
        {
            Enum.TryParse(roleName, out Role role);
            user.AddRole(role);
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            Enum.TryParse(roleName, out Role role);
            user.RemoveRole(role);
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return Task.FromResult((IList<string>)user.GetRoles());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            Enum.TryParse(roleName, out Role role);
            return Task.FromResult(user.IsInRole(role));
        }
        #endregion

        //return default values or throws NotImplementedException
        #region IUserLockoutStore
        public Task<bool> GetLockoutEnabledAsync(User user)
        => Task.FromResult(false);
        public Task<int> GetAccessFailedCountAsync(User user)
        => Task.FromResult(0);

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user) => throw new NotImplementedException();
        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd) => throw new NotImplementedException();
        public Task ResetAccessFailedCountAsync(User user) => throw new NotImplementedException();
        public Task<int> IncrementAccessFailedCountAsync(User user) => throw new NotImplementedException();
        public Task SetLockoutEnabledAsync(User user, bool enabled) => throw new NotImplementedException();
        #endregion

        //return default value or do nothing
        #region IUserTwoFactorStore
        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        => Task.CompletedTask;

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        => Task.FromResult(false);
        #endregion
    }
}
