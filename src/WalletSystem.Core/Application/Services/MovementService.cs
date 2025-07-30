using AutoMapper;
using WalletSystem.Core.Application.DTOs.Movement;
using WalletSystem.Core.Application.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MovementService(
        IMovementRepository movementRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _movementRepository = movementRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Movement>> GetByWalletAsync(int walletId)
        => await _movementRepository.GetByWalletIdAsync(walletId);

    public async Task<MovementDto> CreateMovementAsync(int walletId, CreateMovementDto dto)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId)
                     ?? throw new WalletNotFoundException(walletId);

        if (dto.Amount <= 0)
            throw new DomainException("Amount must be > 0");

        if (dto.RelatedWalletId.HasValue) //
        {
            throw new DomainException("For inter-wallet transfers, please use the '/api/movements/transfer' endpoint. This endpoint is for simple deposits or withdrawals.");
        }

        if (dto.Type == MovementType.Debit && wallet.Balance < dto.Amount)
            throw new InsufficientBalanceException(walletId);

        var movement = _mapper.Map<Movement>(dto);
        movement.WalletId = walletId;
        movement.CreatedAt = DateTime.UtcNow;

        wallet.Balance += dto.Type == MovementType.Credit ? dto.Amount : -dto.Amount;

        await _walletRepository.UpdateAsync(wallet);
        await _movementRepository.AddAsync(movement);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MovementDto>(movement);
    }

    public async Task TransferAsync(int fromWalletId, TransferDto dto)
    {
        if (dto.Amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        if (fromWalletId == dto.ToWalletId)
            throw new DomainException("Source and destination wallets must differ.");

        // Fetch both wallets
        var from = await _walletRepository.GetByIdAsync(fromWalletId)
                   ?? throw new WalletNotFoundException(fromWalletId);

        var to = await _walletRepository.GetByIdAsync(dto.ToWalletId)
                 ?? throw new WalletNotFoundException(dto.ToWalletId);

        if (from.Balance < dto.Amount)
            throw new InsufficientBalanceException(fromWalletId);

        // Ensure atomicity
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Debit sender
            await _movementRepository.AddAsync(new Movement
            {
                WalletId = fromWalletId,
                RelatedWalletId = dto.ToWalletId,
                Amount = dto.Amount,
                Type = MovementType.Debit,
                CreatedAt = DateTime.UtcNow
            });

            // Credit receiver
            await _movementRepository.AddAsync(new Movement
            {
                WalletId = dto.ToWalletId,
                RelatedWalletId = fromWalletId,
                Amount = dto.Amount,
                Type = MovementType.Credit,
                CreatedAt = DateTime.UtcNow
            });

            // Update balances
            from.Balance -= dto.Amount;
            to.Balance += dto.Amount;

            await _walletRepository.UpdateAsync(from);
            await _walletRepository.UpdateAsync(to);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}