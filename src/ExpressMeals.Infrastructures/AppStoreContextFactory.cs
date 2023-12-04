using ExpressMeals.Infrastructures.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExpressMeals.Infrastructures;

public class AppStoreContextFactory : IDesignTimeDbContextFactory<AppDataContext>
{
    
    public AppDataContext CreateDbContext(string[] args)
    {
        var myConnection = "Server=localhost;User=root;Port=3306;Password=dio313;Database=ExpressMealsDb";

        var optionsBuilder = new DbContextOptionsBuilder<AppDataContext>();
        optionsBuilder.UseMySql(myConnection, MariaDbServerVersion.LatestSupportedServerVersion);

        return new AppDataContext(optionsBuilder.Options);
    }
}