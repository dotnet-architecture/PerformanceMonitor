using System.Data.Entity;
using PerfMonitor;


public class CPUContext : DbContext
{
    public DbSet<CPU_Usage> CPU_Data { get; set; }
}
