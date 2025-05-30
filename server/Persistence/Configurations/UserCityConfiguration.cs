using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Models;

namespace Persistence.Configurations;

public class UserCityConfiguration: IEntityTypeConfiguration<UserCity>
{
    public void Configure(EntityTypeBuilder<UserCity> builder)
    {
        builder.ToTable("userCities"); 

        builder.HasKey(uc => new { uc.UserId, uc.CityId });

        builder.Property(uc => uc.CityName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(uc => uc.IsSelected)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("false"); 

        // Настройка связи с User
        builder.HasOne(uc => uc.User)
            .WithMany() 
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}