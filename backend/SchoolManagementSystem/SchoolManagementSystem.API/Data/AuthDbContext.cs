using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Models;

public class AuthDbContext : IdentityDbContext<IdentityUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Roles (Student, Parent, Teacher, Admin)

        // Role IDs
        var adminRoleId = "4bc237d5-dcbf-40d1-b76a-11302351bc8d";
        var teacherRoleId = "fa6e104c-a16d-494f-95ff-ab8b8b52393f";
        var studentRoleId = "e9d88cfc-d375-4ec7-9081-55e076f09652";
        var parentRoleId = "442eb6ff-246e-4609-b099-faa1b1d101f0";

        // Roles

        var roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = adminRoleId
            },
            new IdentityRole
            {
                Id = teacherRoleId,
                Name = "Teacher",
                NormalizedName = "TEACHER",
                ConcurrencyStamp = teacherRoleId
            },
              new IdentityRole
            {
                Id = studentRoleId,
                Name = "Student",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = studentRoleId
            },
              new IdentityRole
              {
                  Id = parentRoleId,
                  Name = "Parent",
                  NormalizedName = "PARENT",
                  ConcurrencyStamp = parentRoleId
              }
        };

        builder.Entity<IdentityRole>().HasData(roles);

        // Admin User
        var adminUserId = "e38fd539-efda-453b-9f5f-9c9b1a384f45";

        var adminUser = new IdentityUser
        {
            Id = adminUserId,
            UserName = "admin@school.com",
            Email = "admin@school.com",
            NormalizedUserName = "admin@school.com".ToUpper(),
            NormalizedEmail = "admin@school.com".ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, Environment.GetEnvironmentVariable("ADMIN__PASSWORD"));

        builder.Entity<IdentityUser>().HasData(adminUser);

        // Assign Admin role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }
}
