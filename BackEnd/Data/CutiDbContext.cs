using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
namespace BackEnd.Data
{
    public class CutiDbContext : DbContext
    {
        public CutiDbContext(DbContextOptions<CutiDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CutiRemaining> CutiRemaining { get; set; }
        public DbSet<Cuti> Cuti { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(b => b.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Cuti>()
                .Property(b => b.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Cuti>()
                .HasOne(s => s.User)
                .WithMany(m => m.Cutis)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.CutiRemaining)
                .WithOne(c => c.User)
                .HasForeignKey<CutiRemaining>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
