using ExpressMeals.Infrastructures.Context;

namespace ExpressMeals.WebServer;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        var serviceScope = host.Services.CreateScope();

        var initializers = serviceScope.ServiceProvider.GetServices<IDataContextSeeder>();

        foreach (var initializer in initializers)
        {
            initializer.Seed();
        }


        return host;
    }
}