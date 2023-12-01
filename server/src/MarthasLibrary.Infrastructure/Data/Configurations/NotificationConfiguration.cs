using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
  public void Configure(EntityTypeBuilder<Notification> builder)
  {
    builder.HasKey(n => n.Id);
    builder.HasOne<Customer>().WithMany().HasForeignKey(n => n.CustomerId);
    builder.HasIndex(n => n.CustomerId); // Index for faster lookups by customer
    builder.HasOne<Book>().WithMany().HasForeignKey(n => n.BookId);
    builder.HasIndex(n => n.BookId); // Index for faster lookups by book
    builder.Property(n => n.NotificationDate).IsRequired();
    builder.Property(n => n.Status).IsRequired();
  }
}