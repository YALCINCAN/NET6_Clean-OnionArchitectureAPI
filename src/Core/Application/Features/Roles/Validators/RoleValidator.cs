using Application.Features.Roles.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Roles.Validators
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required");
        }
    }

    public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Role ID is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required");
        }
    }
}
