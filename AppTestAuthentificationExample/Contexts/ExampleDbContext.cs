using AppTestAuthentificationExample.Entities;
using InoAuthentification.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppTestAuthentificationExample.Contexts
{
    public partial class ExampleDbContext : InoAuthentificationDbContext
    {
        public ExampleDbContext()
        {

        }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Departement> Departement { get; set; }
        public virtual DbSet<Pays> Pays { get; set; }
        public virtual DbSet<Ville> Ville { get; set; }
        public virtual DbSet<Fournisseur> Fournisseurs { get; set; }
        public virtual DbSet<Client> Client { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Departement>(entity =>
            {
                entity.ToTable("departement", "projet_base");

                entity.HasIndex(e => e.DepartementCode)
                    .HasName("departement_code");

                entity.HasIndex(e => e.IdPays)
                    .HasName("fk_pays_departement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.DepartementCode)
                    .HasColumnName("departement_code")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DepartementNom)
                    .HasColumnName("departement_nom")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IdPays)
                    .HasColumnName("id_pays")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Pays>(entity =>
            {
                entity.ToTable("pays", "projet_base");

                entity.HasIndex(e => e.Code)
                    .HasName("code_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("int(3)");

                entity.Property(e => e.NomEnGb)
                    .IsRequired()
                    .HasColumnName("nom_en_gb")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.NomFrFr)
                    .IsRequired()
                    .HasColumnName("nom_fr_fr")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ville>(entity =>
            {
                entity.ToTable("ville", "projet_base");

                entity.HasIndex(e => e.IdDepartement)
                    .HasName("fk_department_ville");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.IdDepartement)
                    .HasColumnName("id_departement")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.VilleNom)
                    .HasColumnName("ville_nom")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.VilleNomReel)
                    .HasColumnName("ville_nom_reel")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.CodePostal)
                    .IsRequired()
                    .HasColumnName("code_postal")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL"); ;

                entity.HasOne(d => d.IdDepartementNavigation)
                    .WithMany(p => p.Ville)
                    .HasForeignKey(d => d.IdDepartement)
                    .HasConstraintName("fk_department_ville");
            });
        }
    }
}
