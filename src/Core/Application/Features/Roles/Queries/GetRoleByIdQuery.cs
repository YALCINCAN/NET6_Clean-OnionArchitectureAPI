using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using MediatR;

namespace Application.Features.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<IResponse>
    {
        public Guid Id { get; set; }

        public GetRoleByIdQuery(Guid id)
        {
            Id = id;
        }

        public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, IResponse>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IMapper _mapper;

            public GetRoleByIdQueryHandler(IRoleRepository roleRepository, IMapper mapper)
            {
                _roleRepository = roleRepository;
                _mapper = mapper;
            }

            public async Task<IResponse> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
            {
                var role = await _roleRepository.GetByIdAsync(request.Id);
                if (role == null)
                {
                    return new ErrorResponse(404, Messages.NotFound);
                }
                var mappedrole = _mapper.Map<RoleDTO>(role);
                return new DataResponse<RoleDTO>(mappedrole, 200);
            }
        }
    }
}