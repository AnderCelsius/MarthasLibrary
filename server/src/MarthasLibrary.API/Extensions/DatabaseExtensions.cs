using MarthasLibrary.API.Configuration;
using MarthasLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Extensions;

public static class DatabaseExtensions
{
  public static void AddDatabase<TDbContext>(
    this IServiceCollection serviceCollection,
    DatabaseConfiguration databaseConfiguration)
    where TDbContext : DbContext
  {
    serviceCollection.AddDbContextFactory<TDbContext>(builder => builder.UseSqlServer(databaseConfiguration.ConnectionString));

    serviceCollection.AddTransient<DatabaseMigrator<TDbContext>>();
  }

  public static Task MigrateDatabaseToLatestVersion<TDbContext>(this IServiceProvider serviceProvider)
    where TDbContext : DbContext
  {
    return serviceProvider.GetRequiredService<DatabaseMigrator<TDbContext>>().MigrateDbToLatestVersion();
  }
}
