using Microsoft.EntityFrameworkCore;
using VehicleTrackingApi.Models;

namespace VehicleTrackingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicule> Vehicules => Set<Vehicule>();
    public DbSet<PositionGps> PositionsGps => Set<PositionGps>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicule>(entity =>
        {
            entity.ToTable("vehicule");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).HasColumnName("id");
            entity.Property(v => v.Immatriculation)
                .HasColumnName("immatriculation")
                .HasMaxLength(20)
                .IsRequired();
            entity.HasIndex(v => v.Immatriculation).IsUnique();
        });

        modelBuilder.Entity<PositionGps>(entity =>
        {
            entity.ToTable("position_gps");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.VehiculeId).HasColumnName("vehicule_id");
            entity.Property(p => p.Latitude).HasColumnName("latitude");
            entity.Property(p => p.Longitude).HasColumnName("longitude");
            entity.Property(p => p.DatePosition).HasColumnName("date_position");

            entity.HasOne(p => p.Vehicule)
                .WithMany(v => v.PositionsGps)
                .HasForeignKey(p => p.VehiculeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
