using Application.Features.Users.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New Password is required")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                   .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                   .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                   .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                   .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required").Equal(x => x.NewPassword).WithMessage("New Password and ConfirmPassword must match");
        }
    }
}
