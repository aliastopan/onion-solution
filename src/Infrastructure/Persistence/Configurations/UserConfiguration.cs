using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Entities.Identity;

namespace Onion.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("dbo.user");

        builder.Property(x => x.Id)
            .HasColumnName("user_id")
            .ValueGeneratedNever()
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasColumnName("username")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.Property(x => x.Password)
            .HasColumnName("password")
            .HasMaxLength(96) // SHA384
            .IsRequired();

        builder.Property(x => x.Salt)
            .HasColumnName("salt")
            .HasMaxLength(8)
            .IsRequired();
    }
}
