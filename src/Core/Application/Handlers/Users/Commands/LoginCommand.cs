using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Handlers.Users.Commands
{
    public class LoginCommand : IRequest<IResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IRoleRepository _roleRepository;
            private readonly ITokenService _tokenService;
            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly IUnitOfWork _unitOfWork;

            public LoginCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository,IUnitOfWork unitOfWork, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
            {
                _userRepository = userRepository;
                _roleRepository = roleRepository;
                _tokenService = tokenService;
                _refreshTokenRepository = refreshTokenRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<IResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(x => x.UserName == request.UserName, noTracking: true);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                if (!user.EmailConfirmed)
                {
                    throw new ApiException(400, Messages.ConfirmYourEmail);
                }
                if (!PasswordHelper.VerifyHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    throw new ApiException(400, Messages.UserNameOrPasswordIsIncorrect);
                }
                var roles = await _roleRepository.GetAllAsync(x => x.UserRoles.Any(y => y.UserId == user.Id));
                List<string> roleNames = new List<string>();
                foreach (var role in roles)
                {
                    roleNames.Add(role.Name);
                }
                var tokendto = _tokenService.CreateToken(user, roleNames);
                var refreshToken = await _refreshTokenRepository.GetAsync(x => x.UserId == user.Id);
                if (refreshToken == null)
                {
                    await _refreshTokenRepository.AddAsync(new RefreshToken { UserId = user.Id, Code = tokendto.RefreshToken, Expiration = tokendto.RefreshTokenExpiration });
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    refreshToken.Code = tokendto.RefreshToken;
                    refreshToken.Expiration = tokendto.RefreshTokenExpiration;
                    await _unitOfWork.SaveChangesAsync();
                }
                return new DataResponse<TokenDTO>(tokendto, 200);
            }
        }
    }
}