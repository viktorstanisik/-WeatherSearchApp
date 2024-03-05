using Microsoft.EntityFrameworkCore;
using WeatherSearchApp_Domain.EntityModels;


namespace WeatherSearchApp_Domain
{
    public class WeatherSearchAppDbContext : DbContext
    {
        public WeatherSearchAppDbContext(DbContextOptions<WeatherSearchAppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<LoggedUserInfo> LoggedUsersInfo { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
            ConfigureLoggedUserInfo(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

        }

        private static void ConfigureLoggedUserInfo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoggedUserInfo>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<LoggedUserInfo>()
                    .Property(u => u.LastLogin)
                    .IsRequired(false);
        }
    }
}
