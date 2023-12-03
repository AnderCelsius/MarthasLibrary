using MarthasLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MarthasLibrary.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
  public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : base(options) { }

  public DbSet<Book> Books => Set<Book>();
  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<Reservation> Reservations => Set<Reservation>();
  public DbSet<Address> Addresses => Set<Address>();
  public DbSet<Notification> Notifications => Set<Notification>();
  public DbSet<Borrow> Borrows => Set<Borrow>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(modelBuilder);
  }
}
