using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfMonitor;


public class CPUContext : DbContext
{
    public CPUContext(DbContextOptions<CPUContext> options) : base(options)
    { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CPUContextEntityTypeConfiguration());
    }
    public DbSet<CPU_Usage> CPU_Data { get; set; }
    public DbSet<Mem_Usage> MEM_Data { get; set; }
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
