using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Application.Interfaces.Services;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;

namespace WalletSystem.Core.Application.Services;

public class WalletService : IWalletService
{
	private readonly IWalletRepository _walletRepository;

	public WalletService(IWalletRepository walletRepository)
	{
		_walletRepository = walletRepository;
	}

	public async Task<Wallet> CreateWalletAsync(CreateWalletDto dto)
	{
		var wallet = new Wallet
		{
			DocumentId = dto.DocumentId,
			Name = dto.Name,
			Balance = 0,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			IsActive = true
		};

		return await _walletRepository.AddAsync(wallet);
	}

    public async Task<Wallet?> GetByIdAsync(int id)
    => await _walletRepository.GetByIdAsync(id);

    public async Task UpdateWalletNameAsync(int id, string newName)
    {
        var wallet = await _walletRepository.GetByIdAsync(id)
                     ?? throw new WalletNotFoundException(id);

        wallet.Name = newName;
        await _walletRepository.UpdateAsync(wallet);
    }
}