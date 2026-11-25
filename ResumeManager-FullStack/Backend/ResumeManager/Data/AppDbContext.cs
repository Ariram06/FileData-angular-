using Microsoft.EntityFrameworkCore;
using ResumeManager.Models;

namespace ResumeManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Resume> Resumes => Set<Resume>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(x => x.ResumeId);
            entity.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            entity.Property(x => x.StoredFileName).HasMaxLength(255).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
        });
    }
}
