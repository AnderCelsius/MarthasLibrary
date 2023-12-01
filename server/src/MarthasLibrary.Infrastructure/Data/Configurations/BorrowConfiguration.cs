using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;
public class BorrowConfiguration : IEntityTypeConfiguration<Borrow>
{
  public void Configure(EntityTypeBuilder<Borrow> builder)
  {
    builder.HasKey(b => b.Id);

    builder.HasOne<Book>()
      .WithMany()
      .HasForeignKey(b => b.BookId);

    builder.HasIndex(b => b.BookId);

    builder.HasOne<Customer>()
      .WithMany()
      .HasForeignKey(b => b.CustomerId);

    builder.HasIndex(b => b.CustomerId);

    builder.Property(b => b.BorrowDate)
      .IsRequired();

    builder.Property(b => b.DueDate)
      .IsRequired();

    builder.Property(b => b.ReturnDate);

    builder.Property(b => b.BorrowDate).HasColumnType("datetimeoffset");
    builder.Property(b => b.DueDate).HasColumnType("datetimeoffset");
    builder.Property(b => b.ReturnDate).HasColumnType("datetimeoffset");
  }
}
