using Microsoft.EntityFrameworkCore;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Infrastructure.Data;

public class WalletSystemDbContext : DbContext
{
    public WalletSystemDbContext(DbContextOptions<WalletSystemDbContext> options)
        : base(options) { }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Movement> Movements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletSystemDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}