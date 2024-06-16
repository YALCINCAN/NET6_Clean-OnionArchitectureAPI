using Application.Constants;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers.Abstract;
using Application.Wrappers.Concrete;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetAuthenticatedUserWithRolesQuery : IRequest<IResponse>, ICacheable
    {
        public Guid UserId { get; set; }

        public bool BypassCache => false;

        public string CacheKey => $"GetAuthenticatedUserWithRoles-{UserId}";

        public GetAuthenticatedUserWithRolesQuery(Guid userId)
        {
            UserId = userId;
        }

        public class GetAuthenticatedUserWithRolesQueryHandler : IRequestHandler<GetAuthenticatedUserWithRolesQuery, IResponse>
        {
            private readonly IUserRepository _userRepository;

            public GetAuthenticatedUserWithRolesQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<IResponse> Handle(GetAuthenticatedUserWithRolesQuery request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ErrorResponse(404, Messages.UserNotFound);
                }
                var userwithroles = await _userRepository.GetUserWithRolesAsync(request.UserId);
                return new DataResponse<UserDTO>(userwithroles, 200);
            }
        }
    }
}