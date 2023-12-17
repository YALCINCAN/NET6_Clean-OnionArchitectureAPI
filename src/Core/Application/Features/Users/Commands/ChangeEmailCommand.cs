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
using System.Text.Json.Serialization;

namespace Application.Features.Users.Commands
{
    public class ChangeEmailCommand : IRequest<IResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public ChangeEmailCommand(Guid userid, string email)
        {
            UserId = userid;
            Email = email;
        }

        public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEmailService _emailService;
            private readonly IRabbitService _rabbitService;
            private readonly RabbitMQSettings _rabbitMQSettings;
            private readonly ICacheService _cacheService;

            public ChangeEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService, ICacheService cacheService, IRabbitService rabbitService, IOptions<RabbitMQSettings> rabbitMQSettings)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _emailService = emailService;
                _cacheService = cacheService;
                _rabbitService = rabbitService;
                _rabbitMQSettings = rabbitMQSettings.Value;
            }

            public async Task<IResponse> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                var email = await _userRepository.GetAsync(x => x.Email == request.Email);
                if (email != null && user.Email != request.Email)
                {
                    throw new ApiException(400, Messages.EmailIsAlreadyExist);
                }
                user.Email = request.Email;
                user.EmailConfirmed = false;
                user.EmailConfirmedCode = null;
                user.EmailConfirmationCode = PasswordHelper.GenerateRandomString(20);
                await _unitOfWork.SaveChangesAsync();
                string link = "http://localhost:5010/api/users/confirmemail/" + user.EmailConfirmationCode;
                //await _emailService.ConfirmationMailAsync(link, request.Email);
                _rabbitService.Publish<ConfirmationMailSendMessage>
                   (new ConfirmationMailSendMessage()
                   { Email = request.Email, Link = link },
                   _rabbitMQSettings.EmailSenderRabbitMQSettings.Exchange_Default,
                   _rabbitMQSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender);
                await _cacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new SuccessResponse(200, Messages.EmailSuccessfullyChangedConfirmYourEmail);
            }
        }
    }
}