using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne<Book>()
          .WithMany()
          .HasForeignKey(r => r.BookId);

        builder.HasIndex(r => r.BookId);

        builder.Property(r => r.BookId)
          .IsRequired();

        builder.Property(r => r.CustomerId)
          .IsRequired();

        builder.HasIndex(r => r.CustomerId);

        builder.Property(r => r.ReservedDate)
          .IsRequired();

        builder.Property(r => r.ExpiryDate)
          .IsRequired();


        builder.Property(r => r.ReservedDate).HasColumnType("datetimeoffset");
        builder.Property(r => r.ExpiryDate).HasColumnType("datetimeoffset");
    }
}