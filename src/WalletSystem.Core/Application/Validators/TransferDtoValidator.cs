using FluentValidation;
using WalletSystem.Core.Application.DTOs.Movement;

namespace WalletSystem.Core.Application.Validators;

public class TransferDtoValidator : AbstractValidator<TransferDto>
{
    public TransferDtoValidator() =>
        RuleFor(x => x.Amount).GreaterThan(0);
}