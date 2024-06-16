using Application.Constants;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands
{
    public class ChangePasswordCommand : IRequest<IResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }


        public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;

            public ChangePasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<IResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ErrorResponse(404, Messages.UserNotFound);
                }
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return new ErrorResponse(400, Messages.PasswordDontMatchWithConfirmation);
                }
                if (!PasswordHelper.VerifyHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    throw new ApiException(400, Messages.CurrentPasswordIsFalse);
                }

                var (passwordHash, passwordSalt) = PasswordHelper.CreateHash(request.NewPassword);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _unitOfWork.SaveChangesAsync();
                return new SuccessResponse(200, Messages.PasswordChangedSuccessfully);
            }
        }
    }
}
