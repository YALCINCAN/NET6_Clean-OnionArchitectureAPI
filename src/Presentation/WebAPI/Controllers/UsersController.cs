using Application.Features.Roles.Queries;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Wrappers.Abstract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Infrastructure.Extensions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new GetAllUsersWithRolesQuery()));
        }


        [HttpGet("confirmemail/{code}")]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new ConfirmEmailCommand(code)));
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateuserrole")]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserCommand command)
        {
            command.UserId = UserId.Value;
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword (ChangePasswordCommand command)
        {
            command.UserId = UserId.Value;
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }
        
        [Authorize]
        [HttpPost("changeemail")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailCommand command)
        {
            command.UserId = UserId.Value;
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpGet("resetpassword/{code}/{email}")]
        public async Task<IActionResult> ResetPassword(string code,string email)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new ResetPasswordCommand(code, email)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userid}")]
        public async Task<IActionResult> DeleteUser(Guid userid)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new RemoveUserCommand(userid)));
        }

    }
}