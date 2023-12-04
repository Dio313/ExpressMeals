using ExpressMeals.Infrastructures.Context;
using ExpressMeals.Infrastructures.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressMeals.Infrastructures;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDataContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                MariaDbServerVersion.LatestSupportedServerVersion));

        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddRoleValidator<RoleValidator<IdentityRole>>()
            .AddEntityFrameworkStores<AppDataContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
