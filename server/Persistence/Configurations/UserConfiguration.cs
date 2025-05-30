using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Models;

namespace Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users"); 

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(255); 

        builder.Property(u => u.Password)
            .HasMaxLength(255); 

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.GoogleId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(u => u.IsGoogleAccount)
            .HasDefaultValue(false); 

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}