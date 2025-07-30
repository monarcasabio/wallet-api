using WalletSystem.Core.Domain.Enums;

namespace WalletSystem.Core.Application.DTOs.Movement;

public record CreateMovementDto(decimal Amount, MovementType Type, int? RelatedWalletId = null);