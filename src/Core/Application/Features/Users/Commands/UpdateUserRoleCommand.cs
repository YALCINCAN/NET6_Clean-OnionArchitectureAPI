using Application.Constants;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class UpdateUserRoleCommand : IRequest<IResponse>
    {
        public Guid UserId { get; set; }
        public Guid[] RoleIds { get; set; }


        public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICacheService _cacheService;

            public UpdateUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _cacheService = cacheService;
            }

            public async Task<IResponse> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUserRolesByUserIdAsync(request.UserId);
                if (user == null)
                {
                    return new ErrorResponse(404, Messages.UserNotFound);
                }
                if (request.RoleIds != null)
                {
                    user.UserRoles = request.RoleIds.Select(roleid => new UserRole
                    {
                        RoleId = roleid
                    }).ToList();
                }
                else
                {
                    user.UserRoles = new List<UserRole>();
                }
                await _unitOfWork.SaveChangesAsync();
                await _cacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new SuccessResponse(200, Messages.UserRolesUpdatedSuccessfully);
            }
        }
    }
}
