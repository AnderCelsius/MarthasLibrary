using MarthasLibrary.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MarthasLibrary.Infrastructure.Data;

public class LibraryDbContext : IdentityDbContext<Customer>
{
  public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : base(options) { }

  public DbSet<Book> Books => Set<Book>();
  public DbSet<Reservation> Reservations => Set<Reservation>();
  public DbSet<Address> Addresses => Set<Address>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(modelBuilder);
  }
}
