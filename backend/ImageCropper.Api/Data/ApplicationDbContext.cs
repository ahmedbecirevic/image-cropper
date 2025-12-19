using ImageCropper.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageCropper.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ConfigurationModel> Configurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ScaleDown)
                .HasColumnType("decimal(3,2)")
                .IsRequired();
            entity.Property(e => e.LogoPosition)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.LogoImageContentType)
                .HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}