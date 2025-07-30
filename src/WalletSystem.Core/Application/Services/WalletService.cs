using AutoMapper;
using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Interfaces.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;

namespace WalletSystem.Core.Application.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WalletService(
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<WalletDto> CreateWalletAsync(CreateWalletDto dto)
    {
        var wallet = _mapper.Map<Wallet>(dto);
        await _walletRepository.AddAsync(wallet);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<WalletDto>(wallet);
    }

    public async Task<Wallet?> GetByIdAsync(int id)
        => await _walletRepository.GetByIdAsync(id);

    public async Task UpdateWalletNameAsync(int id, string newName)
    {
        var wallet = await _walletRepository.GetByIdAsync(id)
                     ?? throw new WalletNotFoundException(id);
        wallet.Name = newName;
        await _walletRepository.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateWalletAsync(int id)
    {
        await _walletRepository.DeactivateAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}