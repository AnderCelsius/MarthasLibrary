using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
  public void Configure(EntityTypeBuilder<Reservation> builder)
  {
    builder.HasKey(r => r.Id);

    builder.Property(r => r.BookId)
      .IsRequired();

    builder.Property(r => r.CustomerId)
      .IsRequired();

    builder.Property(r => r.CreatedAt)
      .IsRequired();

    builder.Property(r => r.UpdatedAt)
      .IsRequired();

    builder.Property(r => r.Status)
      .IsRequired()
      .HasConversion(
        v => v.ToString(),
        v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v));

    builder.Property(r => r.CreatedAt).HasColumnType("datetimeoffset");
    builder.Property(r => r.UpdatedAt).HasColumnType("datetimeoffset");
  }
}