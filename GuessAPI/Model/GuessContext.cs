using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GuessAPI.Model
{
    public partial class GuessContext : DbContext
    {
        public GuessContext()
        {
        }

        public GuessContext(DbContextOptions<GuessContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LeaderBoard> LeaderBoard { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Transcription> Transcription { get; set; }
        public virtual DbSet<Video> Video { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:nzmsasakyawira.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=sakyawira;Password=__Naruto55__;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<LeaderBoard>(entity =>
            {
                entity.HasKey(e => e.PlayerId)
                    .HasName("PK__LeaderBo__4A4E74C81378B40B");

                entity.Property(e => e.PlayerName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.ToTable("students");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("date_created")
                    .HasColumnType("date");

                entity.Property(e => e.Dob)
                    .HasColumnName("dob")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Transcription>(entity =>
            {
                entity.Property(e => e.Phrase)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.Transcription)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("VideoId");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.Property(e => e.IsFavourite).HasColumnName("isFavourite");

                entity.Property(e => e.ThumbnailUrl)
                    .IsRequired()
                    .HasColumnName("ThumbnailURL")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.VideoTitle)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.WebUrl)
                    .IsRequired()
                    .HasColumnName("WebURL")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
        }
    }
}
