using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Interfaces.Services;
using Application.Wrappers.Abstract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Infrastructure.Extensions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;

        public AuthController(IMediator mediator, ICacheService cacheService)
        {
            _mediator = mediator;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUserWithRolesAsync()
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new GetAuthenticatedUserWithRolesQuery(UserId.Value)));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(CreateTokenByRefreshTokenCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }
    }
}