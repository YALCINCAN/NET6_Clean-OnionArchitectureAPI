using Application.Dtos;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using MediatR;

namespace Application.Handlers.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<IResponse>
    {
        public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IResponse>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IMapper _mapper;

            public GetAllRolesQueryHandler(IRoleRepository roleRepository, IMapper mapper)
            {
                _roleRepository = roleRepository;
                _mapper = mapper;
            }

            public async Task<IResponse> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
            {
                var roles = await _roleRepository.GetAllAsync();
                var mappedroles = _mapper.Map<IEnumerable<RoleDTO>>(roles);
                return new DataResponse<IEnumerable<RoleDTO>>(mappedroles, 200);
            }
        }
    }
}