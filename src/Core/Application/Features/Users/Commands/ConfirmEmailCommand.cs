using Application.Constants;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class ConfirmEmailCommand : IRequest<IResponse>
    {
        public string EmailConfirmationCode { get; set; }

        public ConfirmEmailCommand(string code)
        {
            EmailConfirmationCode = code;
        }
        public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;

            public ConfirmEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<IResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(x => x.EmailConfirmationCode == request.EmailConfirmationCode || x.EmailConfirmedCode == request.EmailConfirmationCode);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                if (user.EmailConfirmed)
                {
                    throw new ApiException(400, Messages.AlreadyEmailConfirmed);
                }
                user.EmailConfirmed = true;
                user.EmailConfirmationCode = null;
                user.EmailConfirmedCode = request.EmailConfirmationCode;
                await _unitOfWork.SaveChangesAsync();
                return new SuccessResponse(200, Messages.SuccessfullyEmailConfirmed);
            }
        }
    }
}