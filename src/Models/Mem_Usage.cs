using System;

namespace PerfMonitor
{
    public class Mem_Usage  // contains the number of bytes used by process, and timestamp
    {
        public long usage { get; set; }
        public DateTime timestamp { get; set; }
    }
}
