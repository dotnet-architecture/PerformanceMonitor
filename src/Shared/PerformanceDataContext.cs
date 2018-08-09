using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataTransfer
{
    public partial class PerformanceDataContext : DbContext
    {
  

        public PerformanceDataContext(DbContextOptions<PerformanceDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contention> Contention { get; set; }
        public virtual DbSet<CPU_Usage> CPU_Usage { get; set; }
        public virtual DbSet<Exceptions> Exceptions { get; set; }
        public virtual DbSet<GC> GC { get; set; }
        public virtual DbSet<Http_Request> Http_Request { get; set; }
        public virtual DbSet<Jit> Jit { get; set; }
        public virtual DbSet<Mem_Usage> MemData { get; set; }
        public virtual DbSet<Session> Session { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=10.0.75.1,1433;Initial Catalog=PerformanceData;Trusted_Connection=false;MultipleActiveResultSets=true;Persist Security Info=false;User Id=sa;Password=Abc12345;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contention>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.Contention)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_Session_ID");
            });

            modelBuilder.Entity<CPU_Usage>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.ToTable("CPU_Usage");

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.usage).HasColumnName("usage");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.CPU_Usage)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_CPU_Session_ID");
            });

            modelBuilder.Entity<Exceptions>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.ToTable("Exception_Data");

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.Exceptions)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_EXCEPTION_Session_ID");
            });

            modelBuilder.Entity<GC>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.ToTable("GC");

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.Gc)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_GC_Session_ID");
            });

            modelBuilder.Entity<Http_Request>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id");



                entity.ToTable("Http_Request");

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.activityID).HasColumnName("activityID");

                entity.Property(e => e.method)
                    .HasColumnName("method")
                    .IsUnicode(false);

                entity.Property(e => e.path)
                    .HasColumnName("path")
                    .IsUnicode(false);

                entity.Property(e => e.type)
                    .HasColumnName("type")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.Http_Request)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_HTTP_Session_ID");
            });

            modelBuilder.Entity<Jit>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.method)
                    .HasColumnName("method")
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.Jit)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_JIT_Session_ID");
            });

            modelBuilder.Entity<Mem_Usage>(entity =>
            {
                entity.HasKey(e => e.timestamp);

                entity.ToTable("MEM_Data");

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppId).HasColumnName("appId");

                entity.Property(e => e.usage).HasColumnName("usage");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.MemData)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("FK_MEM_Session_ID");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.application)
                    .HasColumnName("application")
                    .IsUnicode(false);

                entity.Property(e => e.os)
                    .HasColumnName("OS")
                    .IsUnicode(false);

                entity.Property(e => e.processorCount).HasColumnName("processorCount");

                entity.Property(e => e.process)
                    .HasColumnName("processs")
                    .IsUnicode(false);

                entity.Property(e => e.sampleRate).HasColumnName("sampleRate");

                entity.Property(e => e.sendRate).HasColumnName("sendRate");
            });
        }
    }
}
