using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
  public void Configure(EntityTypeBuilder<Customer> builder)
  {
    builder.Property(c => c.FirstName)
      .IsRequired()
      .HasMaxLength(50);

    builder.Property(c => c.LastName)
      .IsRequired()
      .HasMaxLength(50);

    builder.Property(c => c.IsActive)
      .IsRequired();

    builder.Property(c => c.IdentityUserId)
      .IsRequired();
    builder.HasIndex(c => c.IdentityUserId)
      .IsUnique();

    builder.Property(c => c.Email).IsRequired();
    builder.HasIndex(c => c.Email)
      .IsUnique();

    builder.HasMany(c => c.Addresses)
      .WithOne()
      .HasForeignKey(nameof(Address.CustomerId))
      .OnDelete(DeleteBehavior.Cascade);

    builder.Property(c => c.CreatedAt)
      .IsRequired();

    builder.Property(c => c.UpdatedAt)
      .IsRequired(false);

    builder.Property(c => c.PhoneNumber);
    builder.HasIndex(c => c.PhoneNumber)
      .IsUnique();
  }
}
