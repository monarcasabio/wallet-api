using WalletSystem.Core.Application.DTOs.Wallet;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Interfaces.Services;

public interface IWalletService
{
    Task<WalletDto> CreateWalletAsync(CreateWalletDto dto);
    Task<Wallet?> GetByIdAsync(int id);
    Task UpdateWalletNameAsync(int id, string newName);
    Task DeactivateWalletAsync(int id);
}