using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Interfaces.Services;

public interface IMovementService
{
    Task<IEnumerable<Movement>> GetByWalletAsync(int walletId);
    Task<MovementDto> CreateMovementAsync(int walletId, CreateMovementDto dto);
    Task TransferAsync(int fromWalletId, TransferDto dto);
}