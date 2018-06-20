using Microsoft.EntityFrameworkCore;
using PerfMonitor;


public class CPUContext : DbContext
{
    public CPUContext(DbContextOptions<CPUContext> options) :base(options)
    { }
    public DbSet<CPU_Usage> CPU_Data { get; set; }
}
