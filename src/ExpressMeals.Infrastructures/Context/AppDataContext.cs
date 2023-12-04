using System.Reflection;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Infrastructures.Identities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressMeals.Infrastructures.Context;

public class AppDataContext : IdentityDbContext<ApplicationUser>
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}