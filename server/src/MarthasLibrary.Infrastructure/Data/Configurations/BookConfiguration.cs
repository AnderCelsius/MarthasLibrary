using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarthasLibrary.Infrastructure.Data.Configurations;
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
  public void Configure(EntityTypeBuilder<Book> builder)
  {
    builder.HasKey(b => b.Id);

    builder.Property(b => b.Title)
      .IsRequired()
      .HasMaxLength(250);

    builder.Property(b => b.Author)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(b => b.Isbn)
      .IsRequired()
      .HasMaxLength(13)
      .IsUnicode(false);

    builder.Property(b => b.PublishedDate)
      .IsRequired();

    builder.Property(b => b.CreatedAt)
      .IsRequired();

    builder.Property(b => b.UpdatedAt)
      .IsRequired();

    builder.Property(b => b.PublishedDate).HasColumnType("datetimeoffset");
  }
}
