using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotelBooking.Domain.Entities;

namespace MotelBooking.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.TotalAmount).HasPrecision(10, 2);
        builder.Property(b => b.TaxAmount).HasPrecision(10, 2);
        builder.Property(b => b.DiscountAmount).HasPrecision(10, 2);
        builder.Property(b => b.ConfirmationNumber).HasMaxLength(30);
        builder.HasIndex(b => b.ConfirmationNumber).IsUnique().HasFilter("[ConfirmationNumber] IS NOT NULL");

        builder.HasOne(b => b.Payment).WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.NightsCount);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.Currency).HasMaxLength(10).HasDefaultValue("USD");
        builder.Property(p => p.TransactionId).HasMaxLength(100);
    }
}
