using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.State).IsRequired().HasMaxLength(100);
        builder.Property(l => l.StateCode).IsRequired().HasMaxLength(5);
        builder.Property(l => l.City).IsRequired().HasMaxLength(100);
        builder.Property(l => l.BranchName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.BranchCode).IsRequired().HasMaxLength(30);
        builder.HasIndex(l => l.BranchCode).IsUnique();
        builder.Property(l => l.Address).IsRequired().HasMaxLength(300);
        builder.Property(l => l.PhoneNumber).HasMaxLength(20);
        builder.Property(l => l.Email).HasMaxLength(150);

        builder.HasMany(l => l.Rooms).WithOne(r => r.Location)
            .HasForeignKey(r => r.LocationId).OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(l => l.Bookings).WithOne(b => b.Location)
            .HasForeignKey(b => b.LocationId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(l => new { l.State, l.City });
        builder.HasIndex(l => l.State);
    }
}
