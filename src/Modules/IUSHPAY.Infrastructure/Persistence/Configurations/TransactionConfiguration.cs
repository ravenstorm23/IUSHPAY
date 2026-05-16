using IUSHPAY.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IUSHPAY.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
	public void Configure(EntityTypeBuilder<Transaction> builder)
	{
		builder.HasKey(t => t.Id);

		builder.Property(t => t.Amount)
			.HasColumnType("numeric(18,2)")
			.IsRequired();

		builder.Property(t => t.Status)
			.IsRequired();

		builder.Property(t => t.Type)
			.IsRequired();

		builder.Property(t => t.Reference)
			.HasMaxLength(500)
			.IsRequired();

		builder.Property(t => t.CreatedAt)
			.IsRequired();
	}
}