using Application.Features.Users.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Validators
{
    public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }
}
