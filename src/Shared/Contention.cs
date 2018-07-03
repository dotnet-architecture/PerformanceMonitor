using System;
using System.ComponentModel.DataAnnotations;

namespace PerfMonitor
{
    public class Contention  // contains the percentage of total CPU usage and DateTime of instant
    {
        public String type { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
