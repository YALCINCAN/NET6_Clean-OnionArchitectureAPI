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
            private readonly IEasyCacheService _easyCacheService;

            public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IEasyCacheService easyCacheService, IMapper mapper)
            {
                _roleRepository = roleRepository;
                _unitOfWork = unitOfWork;
                _easyCacheService = easyCacheService;
                _mapper = mapper;
            }

            public async Task<IResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                var existrole = await _roleRepository.GetAsync(x => x.Name == request.Name);
                if (existrole != null)
                {
                    throw new ApiException(400, Messages.RoleNameAlreadyExist);
                }
                var role = await _roleRepository.AddAsync(new Role()
                {
                    Name = request.Name
                });
                await _unitOfWork.SaveChangesAsync();
                await _easyCacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                var mappedrole = _mapper.Map<RoleDTO>(role);
                return new DataResponse<RoleDTO>(mappedrole, 200, Messages.AddedSuccesfully);
            }
        }
    }
}