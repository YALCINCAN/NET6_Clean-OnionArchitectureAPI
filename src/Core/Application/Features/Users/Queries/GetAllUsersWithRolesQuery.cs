using Application.Dtos;
using Application.Interfaces.Repositories;
using Application.Wrappers.Concrete;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Queries
{
    public class GetAllUsersWithRolesQuery : IRequest<DataResponse<IEnumerable<UserDTO>>>
    {
        public class GetAllUsersWithRolesQueryHandler : IRequestHandler<GetAllUsersWithRolesQuery, DataResponse<IEnumerable<UserDTO>>>
        {
            private readonly IUserRepository _userRepository;
            private readonly ILogger<GetAllUsersWithRolesQueryHandler> _logger;

            public GetAllUsersWithRolesQueryHandler(IUserRepository userRepository, ILogger<GetAllUsersWithRolesQueryHandler> logger)
            {
                _userRepository = userRepository;
                _logger = logger;
            }

            public async Task<DataResponse<IEnumerable<UserDTO>>> Handle(GetAllUsersWithRolesQuery request, CancellationToken cancellationToken)
            {
                var userswithroles = await _userRepository.GetAllUsersWithRolesAsync();
                _logger.LogInformation("GetAllUserWithRoles = {@GetAllUserWithRoles}",userswithroles);
                return new DataResponse<IEnumerable<UserDTO>>(userswithroles, 200);
            }
        }
    }
}