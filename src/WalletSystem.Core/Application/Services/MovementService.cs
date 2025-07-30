using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Interfaces.Services;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Services;
public class MovementService : IMovementService
{
    private readonly IMovementRepository _movementRepository;

    public MovementService(IMovementRepository movementRepository)
    {
        _movementRepository = movementRepository;
    }

    public async Task<IEnumerable<Movement>> GetByWalletAsync(int walletId)
    => await _movementRepository.GetByWalletIdAsync(walletId);

}