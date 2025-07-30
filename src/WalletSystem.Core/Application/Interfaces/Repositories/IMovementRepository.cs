using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Enums;

namespace WalletSystem.Core.Application.Interfaces.Repositories;

public interface IMovementRepository
{
    Task<Movement> AddAsync(Movement movement);

    // Si movementType es null, trae ambos tipos
    Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId);
}