using Application.Constants;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.Users.Commands
{
    public class RemoveUserCommand:IRequest<IResponse>
    {
        public Guid UserId { get; set; }

        public RemoveUserCommand(Guid userid)
        {
            UserId = userid;
        }

        public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEasyCacheService _easyCacheService;

            public RemoveUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEasyCacheService easyCacheService)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _easyCacheService = easyCacheService;
            }

            public async Task<IResponse> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                _userRepository.Remove(user);
                await _unitOfWork.SaveChangesAsync();
                await _easyCacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new SuccessResponse(200,Messages.DeletedSuccessfully);
            }
        }
    }
}
