#nullable disable

using Microsoft.EntityFrameworkCore;

namespace KBC.Model
{
    public partial class KBCGrupaContext : DbContext
    {
        public KBCGrupaContext(DbContextOptions<KBCGrupaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pacijent> Pacijenti { get; set; }
        public virtual DbSet<SifDijagnozaMKB10> SifDijagnozaMKB10 { get; set; }
        public virtual DbSet<Pregled> Pregledi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Pacijent>(entity =>
            {
                entity.HasKey("MBO")
                    .HasName("pacijent_pkey");
                
                entity.ToTable("pacijent");
                
                entity.Property(pacijent => pacijent.MBO)
                    .IsRequired()
                    .HasMaxLength(9)
                    .HasColumnName("MBO");

                entity.Property(pacijent => pacijent.Ime)
                    .IsRequired()
                    .HasColumnName("ime");

                entity.Property(pacijent => pacijent.Prezime)
                    .IsRequired()
                    .HasColumnName("prezime");

                entity.Property(pacijent => pacijent.DatumRodjenja)
                    .IsRequired()
                    .HasColumnName("datum_rodenja");

                entity.HasMany(p => p.PovijestPregledas)
                    .WithOne(p => p.Pacijent);
            });

            modelBuilder.Entity<SifDijagnozaMKB10>(entity =>
            {
              
                entity.ToTable("dijagnoza");

                entity.HasKey("mkb10")
                    .HasName("dijagnoza_pkkey");

                entity.Property(dijagnoza => dijagnoza.mkb10)
                    .IsRequired()
                    .HasColumnName("mkb10");

                entity.Property(dijagnoza => dijagnoza.Dijagnoza)
                    .HasColumnName("dijagnoza");

                entity.HasMany(d => d.PovijestPregledas)
                    .WithOne(p => p.Dijagnoza);
            });

            modelBuilder.Entity<Pregled>(entity =>
            {
                // entity.HasKey("id");
                
                entity.ToTable("pregled");

                entity.Property(pregled => pregled.Id)
                    .IsRequired()
                    .HasColumnName("id");

                entity.Property(pregled => pregled.Anamneza)
                    .IsRequired()
                    .HasColumnName("anamneza");

                entity.Property(pregled => pregled.DatumPregleda)
                    .IsRequired()
                    .HasColumnName("datum_pregleda");

                entity.Property(pregled => pregled.PacijentMbo)
                    .IsRequired()
                    .HasColumnName("mbo");
                
                entity.Property(pregled => pregled.DijagnozaMkb10)
                    // .IsRequired()
                    .HasColumnName("mkb10");

                entity.Property(pregled => pregled.Terapija)
                    .IsRequired()
                    .HasColumnName("terapija");

                entity.HasOne(p => p.Dijagnoza)
                    .WithMany(d => d.PovijestPregledas);

                entity.HasOne(p => p.Pacijent)
                    .WithMany(p => p.PovijestPregledas);
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}