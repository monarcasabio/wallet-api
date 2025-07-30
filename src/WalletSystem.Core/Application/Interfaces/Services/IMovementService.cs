using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Interfaces.Services;

public interface IMovementService
{
    Task<IEnumerable<Movement>> GetByWalletAsync(int walletId);
}