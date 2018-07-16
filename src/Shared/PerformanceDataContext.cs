#if TEST_EF
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataTransfer
{
    public partial class PerformanceDataContext : DbContext
    {
        public PerformanceDataContext()
        {
        }

        public PerformanceDataContext(DbContextOptions<PerformanceDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationData> ApplicationData { get; set; }
        public virtual DbSet<ContentionData> ContentionData { get; set; }
        public virtual DbSet<CpuData> CpuData { get; set; }
        public virtual DbSet<ExceptionData> ExceptionData { get; set; }
        public virtual DbSet<GcData> GcData { get; set; }
        public virtual DbSet<HttpData> HttpData { get; set; }
        public virtual DbSet<JitData> JitData { get; set; }
        public virtual DbSet<MemData> MemData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=10.0.75.1,1433;Initial Catalog=PerformanceData;Trusted_Connection=false;MultipleActiveResultSets=true;Persist Security Info=false;User Id=sa;Password=Abc12345;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationData>(entity =>
            {
                entity.ToTable("Application_Data");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Application)
                    .HasColumnName("application")
                    .IsUnicode(false);

                entity.Property(e => e.Process)
                    .HasColumnName("process")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ContentionData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("Contention_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.ContentionData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contention_Data_App");
            });

            modelBuilder.Entity<CpuData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("CPU_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Usage).HasColumnName("usage");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.CpuData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CPU_Data_App");
            });

            modelBuilder.Entity<ExceptionData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("Exception_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.ExceptionData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exception_Data_App");
            });

            modelBuilder.Entity<GcData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("GC_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.GcData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GC_Data_App");
            });

            modelBuilder.Entity<HttpData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("HTTP_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Method)
                    .HasColumnName("method")
                    .IsUnicode(false);

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.HttpData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HTTP_Data_App");
            });

            modelBuilder.Entity<JitData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("JIT_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Method)
                    .HasColumnName("method")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.JitData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JIT_Data_App");
            });

            modelBuilder.Entity<MemData>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("MEM_Data");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Usage).HasColumnName("usage");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.MemData)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MEM_Data_App");
            });
        }
    }
}
#endif
