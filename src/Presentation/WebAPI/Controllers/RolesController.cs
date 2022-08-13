using Application.Features.Roles.Commands;
using Application.Handlers.Roles.Commands;
using Application.Handlers.Roles.Queries;
using Application.Wrappers.Abstract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IResponse> GetRoles()
        {
            return await _mediator.Send(new GetAllRolesQuery());
        }

        [HttpGet("{id}")]
        public async Task<IResponse> GetRole(Guid id)
        {
            return await _mediator.Send(new GetRoleByIdQuery(id));
        }

        [HttpPost]
        public async Task<IResponse> CreateRole(CreateRoleCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut]
        public async Task<IResponse> UpdateRole(UpdateRoleCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<IResponse> RemoveRole(Guid id)
        {
            return await _mediator.Send(new RemoveRoleCommand(id));
        }

        [HttpGet("getrolesbyuserid/{userid}")]
        public async Task<IResponse> GetRolesByUserId(Guid userid)
        {
            return await _mediator.Send(new GetRolesByUserIdQuery(userid));
        }
    }
}