using IUSHPAY.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IUSHPAY.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.InstitutionalCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.Role).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasOne(x => x.Wallet).WithOne().HasForeignKey<Wallet>(w => w.UserId);
    }
}
