using Application.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role, Guid>
    {
        Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId);
    }
}
        