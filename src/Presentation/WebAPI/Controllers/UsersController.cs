using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Application.Wrappers.Abstract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IResponse> GetAllUsersWithRoles()
        {
            return await _mediator.Send(new GetAllUsersWithRolesQuery());
        }


        [HttpGet("confirmemail/{code}")]
        public async Task<IResponse> ConfirmEmail(string code)
        {
            return await _mediator.Send(new ConfirmEmailCommand(code));
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateuserrole")]
        public async Task<IResponse> UpdateUserRole(UpdateUserRoleCommand command)
        {
            return await _mediator.Send(command);
        }

        [Authorize]
        [HttpPut]
        public async Task<IResponse> UpdateUser(UpdateUserCommand command)
        {
            command.UserId = UserId.Value;
            return await _mediator.Send(command);
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IResponse> ChangePassword (ChangePasswordCommand command)
        {
            command.UserId = UserId.Value;
            return await _mediator.Send(command);
        }
        
        [Authorize]
        [HttpPost("changeemail")]
        public async Task<IResponse> ChangeEmail(ChangeEmailCommand command)
        {
            command.UserId = UserId.Value;
            return await _mediator.Send(command);
        }

        [HttpPost("forgetpassword")]
        public async Task<IResponse> ForgetPassword(ForgetPasswordCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpGet("resetpassword/{code}/{email}")]
        public async Task<IResponse> ResetPassword(string code,string email)
        {
            return await _mediator.Send(new ResetPasswordCommand(code, email));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userid}")]
        public async Task<IResponse> DeleteUser(Guid userid)
        {
            return await _mediator.Send(new RemoveUserCommand(userid));
        }

    }
}