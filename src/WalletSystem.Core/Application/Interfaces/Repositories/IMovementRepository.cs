using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Interfaces.Repositories;

public interface IMovementRepository
{
    Task<Movement> AddAsync(Movement movement);
    Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId);
}