using Microsoft.EntityFrameworkCore;
using BookieDookie.Models;

namespace BookieDookie.Data;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<ReadingStats> ReadingStats { get; set; }

    public DbSet<ReadingHistory> ReadingHistory { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =========================
        // USER → BOOKS
        // =========================

        modelBuilder.Entity<Book>()
            .HasOne(b => b.User)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // USER → READING STATS
        // =========================

        modelBuilder.Entity<ReadingStats>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // =========================
        // USER → READING HISTORY
        // =========================

        modelBuilder.Entity<ReadingHistory>()
            .HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // =========================
        // ONE HISTORY RECORD
        // PER USER PER DAY
        // =========================

        modelBuilder.Entity<ReadingHistory>()
            .HasIndex(h => new
            {
                h.UserId,
                h.ReadingDate
            })
            .IsUnique();
    }
}