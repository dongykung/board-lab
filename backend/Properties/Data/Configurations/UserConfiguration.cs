using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BoardApi.Models;

namespace BoardApi.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(u => u.LoginId)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(u => u.LoginId)
            .IsUnique();
    }
}
