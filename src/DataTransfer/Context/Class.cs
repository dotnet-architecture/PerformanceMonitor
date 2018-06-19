using System.Data.Entity;

namespace PerfMonitor
{
    public class CPUContext : DbContext
    {
        public DbSet<CPU_Usage> CPU_Data { get; set; }
    }
}
