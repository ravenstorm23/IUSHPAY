using IUSHPAY.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IUSHPAY.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
	public void Configure(EntityTypeBuilder<Wallet> builder)
	{
		builder.HasKey(w => w.Id);

		builder.Property(w => w.Balance)
			.HasColumnType("numeric(18,2)")
			.IsRequired();

		builder.Property(w => w.UpdatedAt)
			.IsRequired();

		// FIX: indica a EF que use el campo privado _transactions
		// como backing field para la colección de transacciones
		builder.Navigation(w => w.Transactions)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasMany(w => w.Transactions)
			.WithOne(t => t.Wallet)
			.HasForeignKey(t => t.WalletId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(w => w.User)
			.WithOne(u => u.Wallet)
			.HasForeignKey<Wallet>(w => w.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}