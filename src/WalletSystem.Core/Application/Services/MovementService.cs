using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Interfaces.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Enums;
using WalletSystem.Core.Domain.Exceptions;

namespace WalletSystem.Core.Application.Services;
public class MovementService : IMovementService
{
    private readonly IMovementRepository _movementRepository;
    private readonly IWalletRepository _walletRepository;

    public MovementService(IMovementRepository movementRepository, IWalletRepository walletRepository)
    {
        _movementRepository = movementRepository;
        _walletRepository = walletRepository;
    }

    public async Task<IEnumerable<Movement>> GetByWalletAsync(int walletId)
    => await _movementRepository.GetByWalletIdAsync(walletId);

    public async Task<MovementDto> CreateMovementAsync(int walletId, CreateMovementDto dto)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId)
                     ?? throw new WalletNotFoundException(walletId);

        if (dto.Amount <= 0) throw new DomainException("Amount must be > 0");

        if (dto.Type == MovementType.Debit && wallet.Balance < dto.Amount)
            throw new InsufficientBalanceException(walletId);

        var movement = new Movement
        {
            WalletId = walletId,
            Amount = dto.Amount,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.Type == MovementType.Credit) wallet.Balance += dto.Amount;
        else wallet.Balance -= dto.Amount;

        await _movementRepository.AddAsync(movement);
        return new MovementDto(movement.Id,
                               movement.WalletId,
                               movement.Amount,
                               movement.Type,
                               movement.CreatedAt);
    }
}