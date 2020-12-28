using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace locker.Models
{
    public partial class Contxtdb : DbContext
    {
        public Contxtdb()
        {
        }

        public Contxtdb(DbContextOptions<Contxtdb> options)
            : base(options)
        {
        }

        public virtual DbSet<Box> Boxs { get; set; }
        public virtual DbSet<Boxtime> Boxtimes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1401;Database=dotnet;User=sa;Password=Parin123; ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Box>(entity =>
            {
                entity.ToTable("boxs");

                entity.Property(e => e.Pin).HasColumnName("pin");
            });

            modelBuilder.Entity<Boxtime>(entity =>
            {
                entity.HasKey(e => e.Timeid)
                    .HasName("PK__boxtimes__964BAC198C8103C8");

                entity.ToTable("boxtimes");

                entity.Property(e => e.Timeid).HasColumnName("timeid");

                entity.Property(e => e.BookingEnd).HasColumnType("datetime");

                entity.Property(e => e.Bookingstart).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
