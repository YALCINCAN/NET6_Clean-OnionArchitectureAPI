using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using MediatR;

namespace Application.Handlers.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<DataResponse<RoleDTO>>
    {
        public Guid Id { get; set; }

        public GetRoleByIdQuery(Guid id)
        {
            Id = id;
        }

        public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, DataResponse<RoleDTO>>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IMapper _mapper;

            public GetRoleByIdQueryHandler(IRoleRepository roleRepository, IMapper mapper)
            {
                _roleRepository = roleRepository;
                _mapper = mapper;
            }

            public async Task<DataResponse<RoleDTO>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
            {
                var role = await _roleRepository.GetByIdAsync(request.Id);
                if (role == null)
                {
                    throw new ApiException(404, Messages.NotFound);
                }
                var mappedrole = _mapper.Map<RoleDTO>(role);
                return new DataResponse<RoleDTO>(mappedrole, 200);
            }
        }
    }
}