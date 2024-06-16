using Application.Features.Roles.Commands;
using Application.Features.Roles.Queries;
using Application.Wrappers.Abstract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Infrastructure.Extensions;

namespace WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseController
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new GetAllRolesQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(Guid id)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new GetRoleByIdQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateRoleCommand command)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new RemoveRoleCommand(id)));
        }

        [HttpGet("getrolesbyuserid/{userid}")]
        public async Task<IActionResult> GetRolesByUserId(Guid userid)
        {
            return this.FromResponse<IResponse>(await _mediator.Send(new GetRolesByUserIdQuery(userid)));
        }
    }
}