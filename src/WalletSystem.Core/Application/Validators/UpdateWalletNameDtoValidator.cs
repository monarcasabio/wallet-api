using FluentValidation;
using WalletSystem.Core.Application.DTOs.Wallet;

namespace WalletSystem.Core.Application.Validators;

public class UpdateWalletNameDtoValidator : AbstractValidator<UpdateWalletNameDto>
{
    public UpdateWalletNameDtoValidator() =>
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
}