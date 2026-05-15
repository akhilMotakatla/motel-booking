using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.PricePerNight).HasPrecision(10, 2);
        builder.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(r => r.RoomNumber).IsUnique();

        builder.HasOne(r => r.RoomType).WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Images).WithOne(i => i.Room)
            .HasForeignKey(i => i.RoomId).OnDelete(DeleteBehavior.Cascade);

        // RoomAmenity is a join table without soft-delete; configure as optional
        // navigation so EF does not emit a query-filter mismatch warning.
        builder.HasMany(r => r.RoomAmenities).WithOne(ra => ra.Room)
            .HasForeignKey(ra => ra.RoomId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Bookings).WithOne(b => b.Room)
            .HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Reviews).WithOne(rv => rv.Room)
            .HasForeignKey(rv => rv.RoomId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoomAmenityConfiguration : IEntityTypeConfiguration<RoomAmenity>
{
    public void Configure(EntityTypeBuilder<RoomAmenity> builder)
    {
        builder.HasKey(ra => new { ra.RoomId, ra.AmenityId });
        builder.HasOne(ra => ra.Amenity).WithMany(a => a.RoomAmenities)
            .HasForeignKey(ra => ra.AmenityId).OnDelete(DeleteBehavior.Cascade);
    }
}
