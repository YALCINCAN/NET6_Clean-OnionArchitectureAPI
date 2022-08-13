using Application.Dtos;
using Application.Interfaces.Repositories;
using Application.Wrappers.Concrete;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetAllUsersWithRolesQuery : IRequest<DataResponse<IEnumerable<UserDTO>>>
    {
        public class GetAllUsersWithRolesQueryHandler : IRequestHandler<GetAllUsersWithRolesQuery, DataResponse<IEnumerable<UserDTO>>>
        {
            private readonly IUserRepository _userRepository;

            public GetAllUsersWithRolesQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<DataResponse<IEnumerable<UserDTO>>> Handle(GetAllUsersWithRolesQuery request, CancellationToken cancellationToken)
            {
                var userswithroles = await _userRepository.GetAllUsersWithRolesAsync();
                return new DataResponse<IEnumerable<UserDTO>>(userswithroles, 200);
            }
        }
    }
}