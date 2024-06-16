using Application.Constants;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using Common.Settings;
using Domain.RabbitMessages;
using MediatR;
using Microsoft.Extensions.Options;

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
            private readonly IRabbitService _rabbitService;
            private readonly RabbitMQSettings _rabbitMQSettings;
            public ForgetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService, IRabbitService rabbitService, IOptions<RabbitMQSettings> rabbitMQSettings)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _emailService = emailService;
                _rabbitService = rabbitService;
                _rabbitMQSettings = rabbitMQSettings.Value;
            }
            public async Task<IResponse> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(x => x.Email == request.Email);
                if (user == null)
                {
                    return new ErrorResponse(404, Messages.UserNotFound);
                }
                user.ResetPasswordCode = PasswordHelper.GenerateRandomString(20);
                await _unitOfWork.SaveChangesAsync();
                string link = "http://localhost:5010/api/users/resetpassword/" + user.ResetPasswordCode + "/" + user.Email;
                _rabbitService.Publish<ForgetPasswordMailSendMessage>
                   (new ForgetPasswordMailSendMessage()
                   { Email = request.Email, Link = link },
                   _rabbitMQSettings.EmailSenderRabbitMQSettings.Exchange_Default,
                   _rabbitMQSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.Queue_ForgetPasswordMailSender);
                //await _emailService.ForgetPasswordMailAsync(link, user.Email);
                return new SuccessResponse(200, Messages.IfEmailTrue);
            }
        }
    }
}
