using FluentValidation;
using WalletSystem.Core.Application.DTOs.Wallet;

namespace WalletSystem.Core.Application.Validators;

public class CreateWalletDtoValidator : AbstractValidator<CreateWalletDto>
{
    public CreateWalletDtoValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("DocumentId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name max 100 chars");
    }
}