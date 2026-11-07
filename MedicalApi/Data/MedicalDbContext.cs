using MedicalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalApi.Data;

public class MedicalDbContext : DbContext
{
    public MedicalDbContext(DbContextOptions<MedicalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Spravka> Spravki => Set<Spravka>();
    public DbSet<SpravkaType> SpravkaTypes => Set<SpravkaType>();
    public DbSet<Medicine> Medicines => Set<Medicine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Spravka>()
            .HasOne(s => s.Patient)
            .WithMany(p => p.Spravki)
            .HasForeignKey(s => s.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Spravka>()
            .HasOne(s => s.SpravkaType)
            .WithMany(t => t.Spravki)
            .HasForeignKey(s => s.SpravkaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Spravka>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SpravkaType>()
            .Property(t => t.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Medicine>()
            .Property(m => m.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SpravkaType>().HasData(
            new SpravkaType
            {
                Id = 1,
                Name = "Справка 083",
                Price = 100,
                ValidityDays = 30,
                RequiresDoctorExamination = true,
                ForChildren = false
            },
            new SpravkaType
            {
                Id = 2,
                Name = "Санитарная книжка",
                Price = 750,
                ValidityDays = 365,
                RequiresDoctorExamination = true,
                ForChildren = false
            },
            new SpravkaType
            {
                Id = 3,
                Name = "Справка в бассейн",
                Price = 450,
                ValidityDays = 180,
                RequiresDoctorExamination = true,
                ForChildren = true
            },
            new SpravkaType
            {
                Id = 4,
                Name = "Справка для спортивной секции",
                Price = 600,
                ValidityDays = 180,
                RequiresDoctorExamination = true,
                ForChildren = true
            });
    }
}
