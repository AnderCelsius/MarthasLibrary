using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
  public void Configure(EntityTypeBuilder<Address> builder)
  {
    builder.HasKey(a => a.Id);
    builder.Property(a => a.Street)
     .IsRequired()
     .HasMaxLength(100);

    builder.Property(a => a.City)
    .IsRequired()
    .HasMaxLength(50);

    builder.Property(a => a.State)
    .IsRequired()
    .HasMaxLength(50);

    builder.Property(a => a.Country)
    .IsRequired()
    .HasMaxLength(50);

    builder.Property(a => a.ZipCode)
    .IsRequired()
    .HasMaxLength(20);

    builder.Property(a => a.CreatedAt)
    .IsRequired();

    builder.Property(a => a.UpdatedAt)
    .IsRequired();

    builder.Property(a => a.CustomerId)
      .IsRequired();

    builder.HasOne<Customer>()
    .WithMany()
    .HasForeignKey(a => a.CustomerId)
    .IsRequired()
    .OnDelete(DeleteBehavior.Cascade);
  }
}
