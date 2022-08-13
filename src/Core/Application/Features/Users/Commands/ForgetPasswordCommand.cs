using Application.Constants;
using Application.Exceptions;
using Application.Helpers;
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

namespace Application.Features.Users.Commands
{
    public class ForgetPasswordCommand : IRequest<IResponse>
    {
        public string Email { get; set; }

        public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEmailService _emailService;
            public ForgetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _emailService = emailService;
            }
            public async Task<IResponse> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(x => x.Email == request.Email);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                user.ResetPasswordCode = PasswordHelper.GenerateRandomString(20);
                await _unitOfWork.SaveChangesAsync();
                string link = "http://localhost:5010/api/users/resetpassword/" + user.ResetPasswordCode + "/" + user.Email;
                await _emailService.ForgetPasswordMailAsync(link, user.Email);
                return new SuccessResponse(200, Messages.IfEmailTrue);
            }
        }
    }
}
