using JWTAuthentication.Data;
using JWTAuthentication.Infrastructures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { Database.EnsureCreated(); }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(

                new Product { Id = 1, Name = "Banana", Price = 120M },
                new Product { Id = 2, Name = "Apple", Price = 100M },
                new Product { Id = 3, Name = "Orange", Price = 80M }
            );

            modelBuilder.Entity<UserRoles>().HasKey(ur=> new {ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur=>ur.Role)
                .WithMany(r=>r.UserRoles)
                .HasForeignKey(ur=>ur.RoleId);

        }

        public static  async Task CreateBasicData(IServiceProvider serviceProvider,
            IConfiguration config)
        {
            ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            IAuthService authService = serviceProvider.GetRequiredService<IAuthService>();

            foreach(var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await dbContext.Roles.AnyAsync(r => r.Name == (Roles)role))
                {
                    await dbContext.Roles.AddAsync(new Role() { Name = (Roles)role });
                }
            }
            await dbContext.SaveChangesAsync();

            if (!dbContext.Users.Any(u=>u.Username == config["Data:AdminUser:Name"]))
            {
                var admin = new RegisterUser
                {
                    Username = config["Data:AdminUser:Name"],
                    Email = config["Data:AdminUser:Email"],
                    Password = config["Data:AdminUser:Password"]
                };

                authService.Register(admin);
            }
            

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
    }
}
