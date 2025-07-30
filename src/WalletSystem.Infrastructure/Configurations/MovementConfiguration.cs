using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WalletSystem.Core.Domain.Entities;

namespace WalletSystem.Infrastructure.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
	public void Configure(EntityTypeBuilder<Movement> builder)
	{
		builder.ToTable("Movements");

		builder.HasKey(m => m.Id);

		builder.Property(m => m.Amount)
			.HasPrecision(18, 2)
			.IsRequired();

		builder.Property(m => m.Type)
			.IsRequired();

		builder.Property(m => m.Description)
			.HasMaxLength(200);

		builder.Property(m => m.CreatedAt)
			.IsRequired();

		builder.HasIndex(m => new { m.WalletId, m.CreatedAt });
		builder.HasIndex(m => m.RelatedWalletId);
	}
}