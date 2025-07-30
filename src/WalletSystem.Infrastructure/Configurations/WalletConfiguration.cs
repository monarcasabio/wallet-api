using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Infrastructure.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedOnAdd();

        builder.Property(w => w.DocumentId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Balance)
            .HasPrecision(18, 2);  

        builder.Property(w => w.IsActive)
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedAt);
        builder.Property(w => w.UpdatedAt);

        builder.HasIndex(w => w.DocumentId)
            .IsUnique();

        // Relaciones
        builder.HasMany(w => w.MovementsFrom)
            .WithOne(m => m.Wallet)
            .HasForeignKey(m => m.WalletId);

        builder.HasMany(w => w.MovementsTo)
            .WithOne(m => m.RelatedWallet)
            .HasForeignKey(m => m.RelatedWalletId);
    }
}