using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class CreateTokenByRefreshTokenCommand : IRequest<IResponse>
    {
        public string RefreshToken { get; set; }

        public class CreateTokenByRefreshTokenHandler : IRequestHandler<CreateTokenByRefreshTokenCommand, IResponse>
        {
            private readonly IUserRepository _userRepository;
            private readonly IRoleRepository _roleRepository;
            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly ITokenService _tokenService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly ICacheService _cacheService;

            public CreateTokenByRefreshTokenHandler(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork, ITokenService tokenService, IRoleRepository roleRepository, ICacheService cacheService)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _tokenService = tokenService;
                _refreshTokenRepository = refreshTokenRepository;
                _roleRepository = roleRepository;
                _cacheService = cacheService;
            }

            public async Task<IResponse> Handle(CreateTokenByRefreshTokenCommand request, CancellationToken cancellationToken)
            {
                var existRefreshToken = await _refreshTokenRepository.GetAsync(x => x.Code == request.RefreshToken);
                if (existRefreshToken == null)
                {
                    throw new ApiException(404, Messages.RefreshTokenNotFound);
                }
                var user = await _userRepository.GetByIdAsync(existRefreshToken.UserId);
                if (user == null)
                {
                    throw new ApiException(404, Messages.UserNotFound);
                }
                if (existRefreshToken.Expiration < DateTime.Now)
                {
                    throw new ApiException(404, Messages.RefreshTokenExpired);
                }
                var roles = await _roleRepository.GetAllAsync(x => x.UserRoles.Any(y => y.UserId == user.Id));
                List<string> roleNames = new List<string>();
                foreach (var role in roles)
                {
                    roleNames.Add(role.Name);
                }
                var tokendto = _tokenService.CreateToken(user, roleNames);
                existRefreshToken.Code = tokendto.RefreshToken;
                existRefreshToken.Expiration = tokendto.RefreshTokenExpiration;
                await _unitOfWork.SaveChangesAsync();
                await _cacheService.RemoveByPrefixAsync("GetAuthenticatedUserWithRoles");
                return new DataResponse<TokenDTO>(tokendto, 200);
            }
        }
    }
}