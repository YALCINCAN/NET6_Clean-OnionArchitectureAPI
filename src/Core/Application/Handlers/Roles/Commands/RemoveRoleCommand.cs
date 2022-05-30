using Application.Constants;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;

namespace Application.Handlers.Roles.Commands
{
    public class RemoveRoleCommand : IRequest<IResponse>
    {
        public Guid Id { get; set; }

        public RemoveRoleCommand(Guid id)
        {
            Id = id;
        }

        public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, IResponse>
        {
            private readonly IRoleRepository _roleRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEasyCacheService _easyCacheService;

            public RemoveRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IEasyCacheService easyCacheService)
            {
                _roleRepository = roleRepository;
                _unitOfWork = unitOfWork;
                _easyCacheService = easyCacheService;
            }

            public async Task<IResponse> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
            {
                var exisrole = await _roleRepository.GetByIdAsync(request.Id);
                if (exisrole == null)
                {
                    throw new ApiException(404, Messages.NotFound);
                }
                _roleRepository.Remove(exisrole);
                await _unitOfWork.SaveChangesAsync();
                await _easyCacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new SuccessResponse(200, Messages.DeletedSuccessfully);
            }
        }
    }
}