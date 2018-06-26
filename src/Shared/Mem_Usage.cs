using System;
using System.ComponentModel.DataAnnotations;

namespace PerfMonitor
{
    public class Mem_Usage  // contains the number of bytes used by process, and timestamp
    {
        public long usage { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
