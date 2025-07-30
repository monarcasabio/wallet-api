using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Core.Application.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id);
    Task<Wallet?> GetByDocumentIdAsync(string documentId);
    Task<Wallet> AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task DeactivateAsync(int id); 
}