using Application.Constants;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using MediatR;

namespace Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<IResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponse>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly IEasyCacheService _easyCacheService;

            public UpdateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IMapper mapper, IEasyCacheService easyCacheService)
            {
                _roleRepository = roleRepository;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _easyCacheService = easyCacheService;
            }


            public async Task<IResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
            {
                var existrole = await _roleRepository.GetByIdAsync(request.Id);
                if (existrole == null)
                {
                    throw new ApiException(404, Messages.NotFound);
                }
                var roleName = await _roleRepository.GetAsync(x => x.Name == request.Name);
                if (roleName != null && existrole.Name != request.Name)
                {
                    throw new ApiException(400, Messages.RoleNameAlreadyExist);
                }
                _mapper.Map(request, existrole);
                await _unitOfWork.SaveChangesAsync();
                await _easyCacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new SuccessResponse(200, Messages.UpdatedSuccessfully);
            }
        }
    }
}