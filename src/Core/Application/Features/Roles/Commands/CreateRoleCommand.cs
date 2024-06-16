using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Roles.Commands
{
    public class CreateRoleCommand : IRequest<IResponse>
    {
        public string Name { get; set; }

        public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, IResponse>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ICacheService _cacheService;

            public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
            {
                _roleRepository = roleRepository;
                _unitOfWork = unitOfWork;
                _cacheService = cacheService;
                _mapper = mapper;
            }

            public async Task<IResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                var existrole = await _roleRepository.GetAsync(x => x.Name == request.Name);
                if (existrole != null)
                {
                    return new ErrorResponse(400, Messages.RoleNameAlreadyExist);
                }
                var role = await _roleRepository.AddAsync(new Role()
                {
                    Name = request.Name
                });
                await _unitOfWork.SaveChangesAsync();
                await _cacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                var mappedrole = _mapper.Map<RoleDTO>(role);
                return new DataResponse<RoleDTO>(mappedrole, 200, Messages.AddedSuccesfully);
            }
        }
    }
}