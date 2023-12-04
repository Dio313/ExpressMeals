using System.Security.Claims;
using ExpressMeals.Contracts.Wrappers;
using ExpressMeals.Domains.Entities;
using ExpressMeals.Infrastructures.Identities;
using Microsoft.AspNetCore.Identity;

namespace ExpressMeals.Infrastructures.Context;

public class DataContextSeeder : IDataContextSeeder
{
    private readonly AppDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DataContextSeeder(AppDataContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Seed()
    {
        AddAdministrator();
        AddBasicUser();
        AddCategorySeed();
        _context.SaveChanges();
    }

    private void AddAdministrator()
    {
        Task.Run(async () =>
        {
            //Check if Role Exists
            //Check if Role Exists
            var adminRole = new IdentityRole(RoleConstants.AdministratorRole);
            var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
            if (adminRoleInDb == null)
            {
                await _roleManager.CreateAsync(adminRole);
            }
            //Check if User Exists
            var adminUser = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = "Admin",
                LastName = "System-Admin",
                Email = "admin@ExpressMeals.com",
                UserName = "admin@ExpressMeals.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var superUserInDb = await _userManager.FindByEmailAsync(adminUser.Email);
            if (superUserInDb == null)
            {
                await _userManager.CreateAsync(adminUser, "Admin@1234");
                var result = await _userManager.AddToRoleAsync(adminUser, RoleConstants.AdministratorRole);
                if (result.Succeeded)
                {
                    await _roleManager.AddClaimAsync(adminRole, new Claim(ClaimTypes.Role, RoleConstants.AdministratorRole));
                }

            }
        }).GetAwaiter().GetResult();
    }


    private void AddBasicUser()
    {
        Task.Run(async () =>
        {
            //Check if Role Exists
            var userRole = new IdentityRole(RoleConstants.UserRole);
            var userRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.UserRole);
            if (userRoleInDb == null)
            {
                await _roleManager.CreateAsync(userRole);
            }
            //Check if User Exists
            var basicUser = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = "User1",
                LastName = "Joh",
                Email = "user@aol.com",
                UserName = "user@aol.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
            if (basicUserInDb == null)
            {
                await _userManager.CreateAsync(basicUser, "User@1234");
                var result = await _userManager.AddToRoleAsync(basicUser, RoleConstants.UserRole);
                if (result.Succeeded)
                {
                    await _roleManager.AddClaimAsync(userRole, new Claim(ClaimTypes.Role, RoleConstants.UserRole));
                }
            }
        }).GetAwaiter().GetResult();
    }


    private void AddCategorySeed()
    {
        Task.Run(async () =>
            {
                if (_context.Categories != null && !_context.Categories.Any())
                {
                    await _context.Categories.AddRangeAsync(new[]
                    {
                        new Category {Name = "BreakFast"},
                        new Category {Name = "FastFood"},
                        new Category {Name = "Beverages"},
                        new Category {Name = "Breads"},
                        new Category {Name = "Cakes"},
                        new Category {Name = "Ice-Creams"},
                        new Category {Name = "Snacks"}
                    });
                    await _context.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
    }

}