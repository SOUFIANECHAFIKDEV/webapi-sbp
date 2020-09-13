
using InoAuthentification.Entities;
using Microsoft.EntityFrameworkCore;

namespace InoAuthentification.DbContexts
{
  

    public partial class InoAuthentificationDbContext : DbContext
    {
        public InoAuthentificationDbContext()
        {
        }

        public InoAuthentificationDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<ProfileAction> ProfileAction { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().HasKey(userProfile => new {
                userProfile.Iduser,
                userProfile.Idprofile
            });

            modelBuilder.Entity<ProfileAction>().HasKey(profileAction => new {
                profileAction.Idprofile,
                profileAction.Idaction
            });

            modelBuilder.Entity<Action>(entity =>
            {


                entity.ToTable("action");

                entity.HasIndex(e => e.IdModule)
                    .HasName("fk_action_module_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CodeName)
                    .IsRequired()
                    .HasColumnName("code_name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.IdModule)
                    .HasColumnName("id_module")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasColumnName("libelle")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdModuleNavigation)
                    .WithMany(p => p.Action)
                    .HasForeignKey(d => d.IdModule)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_action_module");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("module");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasColumnName("libelle")
                    .HasMaxLength(70)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("profile");

                entity.HasIndex(e => e.Libelle)
                    .HasName("libelle_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasColumnName("libelle")
                    .HasMaxLength(190)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProfileAction>(entity =>
            {
                entity.ToTable("profile_action");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idaction)
                    .HasColumnName("idaction")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idprofile)
                    .HasColumnName("idprofile")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Username)
                    .HasName("username_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Accessfailedcount)
                    .HasColumnName("accessfailedcount")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Actif)
                    .HasColumnName("actif")
                    .HasColumnType("SMALLINT(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Dernierecon)
                    .HasColumnName("dernierecon")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");


                entity.Property(e => e.JoinDate).HasColumnName("join_date");

                entity.Property(e => e.Nom)
                    .HasColumnName("nom")
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Passwordhash)
                    .HasColumnName("passwordhash")
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Phonenumber)
                    .HasColumnName("phonenumber")
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Prenom)
                    .HasColumnName("prenom")
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");



                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(190)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => new { e.Iduser, e.Idprofile });

                entity.ToTable("user_profile");

                entity.HasIndex(e => e.Idprofile)
                    .HasName("fk_user_profile_profile_idx");

                entity.Property(e => e.Iduser)
                    .HasColumnName("iduser")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idprofile)
                    .HasColumnName("idprofile")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdprofileNavigation)
                    .WithMany(p => p.UserProfile)
                    .HasForeignKey(d => d.Idprofile)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_profile_profile");
            });

        }

    }

}
