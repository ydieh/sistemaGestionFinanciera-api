using BancoSol.Domain.Entidades;
using BancoSol.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BancoSol.Infrastructure.Datos;

public class BancoSolDbContext : DbContext
{
    public BancoSolDbContext(DbContextOptions<BancoSolDbContext> options) : base(options) { }

    public DbSet<Transaccion> Transacciones => Set<Transaccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaccion>(entidad =>
        {
            entidad.ToTable("Transacciones");

            entidad.HasKey(t => t.Id);

            entidad.Property(t => t.Monto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entidad.Property(t => t.Descripcion)
                .HasMaxLength(500)
                .IsRequired();

            entidad.Property(t => t.Fecha)
                .IsRequired();

            entidad.Property(t => t.Origen)
                .HasMaxLength(100)
                .IsRequired();

            entidad.Property(t => t.Tipo)
                .HasConversion<string>() 
                .HasMaxLength(20)
                .IsRequired();

            entidad.Property(t => t.CreadoPor)
                .HasMaxLength(200)
                .IsRequired();

            entidad.Property(t => t.FechaCreacion)
                .IsRequired();

      
            entidad.Property(t => t.Moneda)
                .HasConversion(
                    moneda => moneda.Codigo,
                    codigo => Moneda.Crear(codigo))
                .HasColumnName("Moneda")
                .HasMaxLength(3)
                .IsRequired();

            entidad.HasIndex(t => t.Fecha); 
        });
    }
}
