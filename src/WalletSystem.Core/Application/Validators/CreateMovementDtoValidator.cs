using FluentValidation;
using WalletSystem.Core.Application.DTOs.Movement;

namespace WalletSystem.Core.Application.Validators;
public class CreateMovementDtoValidator : AbstractValidator<CreateMovementDto>
{
    public CreateMovementDtoValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Type).IsInEnum();
    }
}