using Microsoft.EntityFrameworkCore;
using Resteurant_API.Entities;

namespace Resteurant_API.DataContext
{
    public class ResteurantDbContext : DbContext
    {
        private string _connectionString = "Server=PGAMORSKI\\SQL2016;Database=ResteurantDb;Trusted_Connection=True;";

        #region DbSets
        public DbSet<Resteurant> Resteurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Resteurant Builder
            modelBuilder.Entity<Resteurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);
            #endregion

            #region Dish Builder
            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();
            #endregion

            #region Address Builder
            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);
            #endregion

            #region User Builder
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();
            #endregion

            #region Role Builder
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
