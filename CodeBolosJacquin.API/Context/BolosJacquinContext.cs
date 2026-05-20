using System;
using System.Collections.Generic;
using CodeBolosJacquin.API.Domains;
using Microsoft.EntityFrameworkCore;

namespace CodeBolosJacquin.API.Context;

public partial class BolosJacquinContext : DbContext
{
    public BolosJacquinContext()
    {
    }

    public BolosJacquinContext(DbContextOptions<BolosJacquinContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bolo> Bolos { get; set; }

    public virtual DbSet<BoloImagen> BoloImagens { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bolo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bolos__3214EC0769BF5A87");

            entity.HasMany(d => d.Categoria).WithMany(p => p.Bolos)
                .UsingEntity<Dictionary<string, object>>(
                    "BoloCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("CategoriaId")
                        .HasConstraintName("FK_BoloCategorias_Categorias"),
                    l => l.HasOne<Bolo>().WithMany()
                        .HasForeignKey("BoloId")
                        .HasConstraintName("FK_BoloCategorias_Bolos"),
                    j =>
                    {
                        j.HasKey("BoloId", "CategoriaId").HasName("PK__BoloCate__F634121BD3A0368B");
                        j.ToTable("BoloCategorias");
                    });
        });

        modelBuilder.Entity<BoloImagen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BoloImag__3214EC07B0EB0536");

            entity.HasOne(d => d.Bolo).WithMany(p => p.BoloImagens).HasConstraintName("FK_BoloImagens_Bolos");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07FC42DA4B");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07F1D03F44");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
