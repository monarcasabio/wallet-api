using WalletSystem.Core.Application.Interfaces.Repositories;
using WalletSystem.Core.Domain.Entities;
using WalletSystem.Core.Domain.Enums;
using WalletSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MovementRepository : IMovementRepository
{
    private readonly WalletSystemDbContext _context;

    public MovementRepository(WalletSystemDbContext context) => _context = context;

    public async Task<IEnumerable<Movement>> GetByWalletIdAsync(int walletId)
        => await _context.Movements
                         .Where(m => m.WalletId == walletId)
                         .OrderByDescending(m => m.CreatedAt)
                         .ToListAsync();

    public async Task<Movement> AddAsync(Movement movement)
    {
        _context.Movements.Add(movement);
        return movement;
    }
}