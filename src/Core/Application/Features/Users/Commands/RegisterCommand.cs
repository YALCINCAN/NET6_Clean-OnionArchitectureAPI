using Application.Constants;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using AutoMapper;
using Common.Settings;
using Domain.Entities;
using Domain.RabbitMessages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Commands
{
    public class RegisterCommand : IRequest<IResponse>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEmailService _emailService;
            private readonly IRabbitService _rabbitService;
            private readonly RabbitMQSettings _rabbitMQSettings;
            private readonly IMapper _mapper;

            public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper, IRabbitService rabbitService, IOptions<RabbitMQSettings> rabbitMQSettings)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _emailService = emailService;
                _mapper = mapper;
                _rabbitService = rabbitService;
                _rabbitMQSettings = rabbitMQSettings.Value;
            }

            public async Task<IResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                // Example transaction usage
                // using var transaction = _unitOfWork.BeginTransaction();
                //try
                //{
                var existuser = await _userRepository.GetAsync(x => x.UserName == request.UserName || x.Email == request.Email, noTracking: true);
                if (existuser?.UserName == request.UserName)
                    return new ErrorResponse(400, Messages.UsernameIsAlreadyExist);

                if (existuser?.Email == request.Email)
                    return new ErrorResponse(400, Messages.EmailIsAlreadyExist);

              
                var (passwordHash, passwordSalt) = PasswordHelper.CreateHash(request.Password);
                var user = _mapper.Map<User>(request);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.EmailConfirmationCode = PasswordHelper.GenerateRandomString(20);
                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                //transaction.Commit();

                //string link = "http://localhost:8080/confirmemail/" + user.EmailConfirmationCode; if u use spa you must use this link example
                string link = "http://localhost:5010/api/users/confirmemail/" + user.EmailConfirmationCode;

                _rabbitService.Publish<ConfirmationMailSendMessage>
                    (new ConfirmationMailSendMessage()
                    { Email = request.Email, Link = link },
                    _rabbitMQSettings.EmailSenderRabbitMQSettings.Exchange_Default,
                    _rabbitMQSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender);
                //await _emailService.ConfirmationMailAsync(link, request.Email);
                return new SuccessResponse(200, Messages.RegisterSuccessfully);
                //}
                //catch (Exception ex)
                //{
                //    transaction.Rollback();
                //    // Logging ...
                //}
            }
        }
    }
}