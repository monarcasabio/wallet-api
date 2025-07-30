using WalletSystem.Core.Domain.Enums;

namespace WalletSystem.Core.Application.DTOs.Movement;

public record MovementDto(int Id, int WalletId, int? RelatedWalletId,
                          decimal Amount, MovementType Type, DateTime CreatedAt);