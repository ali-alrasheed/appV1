using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.seedConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersRepository(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public async Task AddNewRole(string RoleName)
        {
            var newRole = new Role
            {
                Name = RoleName,
                Description = $"This Role for {RoleName}s Users"
            };
            context.Roles.Add(newRole);
            await context.SaveChangesAsync();
        }

        public async Task AddUserToRole(Guid userId, string roleName)
        {
            // الحصول على المستخدم باستخدام UserManager
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new Exception($"User with ID {userId} not found.");

            // التحقق من وجود الدور باستخدام RoleManager
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                throw new Exception($"Role '{roleName}' does not exist.");

            // التحقق ما إذا كان المستخدم لديه الدور مسبقًا
            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (isInRole)
                throw new Exception($"User is already in role '{roleName}'.");

            // إضافة المستخدم إلى الدور
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception($"Failed to add user to role. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task DeleteRole(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if(role is null)
                throw new Exception($"There is no role with {roleId}");
            await _roleManager.DeleteAsync(role);
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) 
                throw new Exception($"There is no user with {userId}");
            await _userManager.DeleteAsync(user);
        }
    }
}
