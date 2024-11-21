using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task AddUserToRole(Guid UserId , string RoleName);
        Task AddNewRole(string RoleName);
        Task DeleteRole(Guid RoleId);
        Task DeleteUser(Guid UserId);

    }
}
