using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using UtilityBill.Models;
namespace UtilityBill.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<TicketDetail> TicketDetail { get; set; }
        public DbSet<ApplicationDetail> ApplicationDetail { get; set; }
        public DbSet<MeterDetail> MeterDetail { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }

        //public ApplicationDbContext(DbContextOptions options) : base(options)
        //{
        //    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        //}
        protected readonly IConfiguration Configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply global query filter to exclude soft-deleted entities
            modelBuilder.Entity<User>().HasQueryFilter(u => !EF.Property<bool>(u, "IsDeleted"));

            // Configure IsDeleted as a shadow property for soft delete on the User entity
            modelBuilder.Entity<User>()
                .Property<bool>("IsDeleted")
                .HasDefaultValue(false);

            // Configure enum mapping to strings for all enum properties
            modelBuilder.Entity<TicketDetail>()
                    .Property(t => t.Type)
                    .HasConversion<string>();

            modelBuilder.Entity<TicketDetail>()
                   .Property(t => t.status)
                   .HasConversion<string>();

            modelBuilder.Entity<User>()
                    .Property(t => t.Role)
                    .HasConversion<string>();

            modelBuilder.Entity<ApplicationDetail>()
                    .Property(t => t.ApplicationStatus)
                    .HasConversion<string>();

            modelBuilder.Entity<ApplicationDetail>()
                   .Property(t => t.ConnectionType)
                   .HasConversion<string>();

            modelBuilder.Entity<ApplicationDetail>()
                   .Property(t => t.RequiredLoad)
                   .HasConversion<string>();

            modelBuilder.Entity<BillDetail>()
                  .Property(t => t.BillStatus)
                  .HasConversion<string>();

            // Define the one-to-many relationship between User and BillDetail
            modelBuilder.Entity<User>()
                .HasMany(u => u.BillDetails)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);
        }
    }

}
