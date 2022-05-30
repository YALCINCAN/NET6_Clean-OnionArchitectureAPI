using Application.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User,Guid>
    {
        Task<UserDTO> GetUserWithRolesAsync(Guid userid);
        Task<IEnumerable<UserDTO>> GetAllUsersWithRolesAsync();
        Task<User> GetUserRolesByUserIdAsync(Guid userid);
    }
}
