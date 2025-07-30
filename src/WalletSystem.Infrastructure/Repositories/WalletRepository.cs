using Microsoft.EntityFrameworkCore;
using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Exceptions;
using WalletSystem.Infrastructure.Data;

namespace WalletSystem.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly WalletSystemDbContext _context;

    public WalletRepository(WalletSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByIdAsync(int id)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);
    }

    public async Task<Wallet?> GetByDocumentIdAsync(string documentId)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.DocumentId == documentId && w.IsActive);
    }

    public async Task<Wallet> AddAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        wallet.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);

        if (wallet == null)
            throw new WalletNotFoundException(id);

        wallet.IsActive = false;
        wallet.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}