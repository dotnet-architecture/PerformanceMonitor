using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DataTransfer;


public class MetricContext : DbContext
{
    public MetricContext(DbContextOptions<MetricContext> options) : base(options)
    { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CPUContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new MEMContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new ExceptionContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new HTTPContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new ContentionContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new GCContextEntityTypeConfiguration());
        builder.ApplyConfiguration(new JitContextEntityTypeConfiguration());
    }
    public DbSet<CPU_Usage> CPU_Data { get; set; }
    public DbSet<Mem_Usage> MEM_Data { get; set; }
    public DbSet<Exceptions> Exception_Data { get; set; }
    public DbSet<Http_Request> HTTP_Data { get; set; }
    public DbSet<Contention> Contention_Data { get; set; }
    public DbSet<Jit> Jit_Data { get; set; }
    public DbSet<GC> GC_Data { get; set; }
}

class CPUContextEntityTypeConfiguration
        : IEntityTypeConfiguration<CPU_Usage>
{
    
    public void Configure(EntityTypeBuilder<CPU_Usage> builder)
    {
        builder.ToTable("CPU_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }
}

class MEMContextEntityTypeConfiguration
        : IEntityTypeConfiguration<Mem_Usage>
{

    public void Configure(EntityTypeBuilder<Mem_Usage> builder)
    {
        builder.ToTable("MEM_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }
}

class ExceptionContextEntityTypeConfiguration 
    : IEntityTypeConfiguration<Exceptions>
{
    public void Configure(EntityTypeBuilder<Exceptions> builder)
    {
        builder.ToTable("Exception_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }

}

class HTTPContextEntityTypeConfiguration
    : IEntityTypeConfiguration<Http_Request>
{
    public void Configure(EntityTypeBuilder<Http_Request> builder)
    {
        builder.ToTable("HTTP_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }

}

class ContentionContextEntityTypeConfiguration
    : IEntityTypeConfiguration<Contention>
{
    public void Configure(EntityTypeBuilder<Contention> builder)
    {
        builder.ToTable("Contention_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }

}

class GCContextEntityTypeConfiguration
    : IEntityTypeConfiguration<GC>
{
    public void Configure(EntityTypeBuilder<GC> builder)
    {
        builder.ToTable("GC_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }

}

class JitContextEntityTypeConfiguration
    : IEntityTypeConfiguration<Jit>
{
    public void Configure(EntityTypeBuilder<Jit> builder)
    {
        builder.ToTable("JIT_Data");

        builder.HasKey(ci => ci.timestamp);
        builder.HasIndex(ci => ci.timestamp);
    }

}