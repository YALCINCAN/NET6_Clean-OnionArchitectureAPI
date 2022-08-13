using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Roles.Queries
{
    public class GetRolesByUserIdQuery : IRequest<DataResponse<IEnumerable<RoleDTO>>>
    {
        public Guid UserId { get; set; }

        public GetRolesByUserIdQuery(Guid userid)
        {
            UserId = userid;
        }

        public class GetRolesByUserIdQueryHandler : IRequestHandler<GetRolesByUserIdQuery, DataResponse<IEnumerable<RoleDTO>>>
        {

            private readonly IRoleRepository _roleRepository;

            public GetRolesByUserIdQueryHandler(IRoleRepository roleRepository)
            {
                _roleRepository = roleRepository;
            }

            public async Task<DataResponse<IEnumerable<RoleDTO>>> Handle(GetRolesByUserIdQuery request, CancellationToken cancellationToken)
            {
                var roles = await _roleRepository.GetRolesByUserIdAsync(request.UserId);
                return new DataResponse<IEnumerable<RoleDTO>>(roles, 200);
            }
        }

    }
}
