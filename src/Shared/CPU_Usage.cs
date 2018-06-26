using System;
using System.ComponentModel.DataAnnotations;

namespace PerfMonitor
{
    public class CPU_Usage  // contains the percentage of total CPU usage and DateTime of instant
    {
        public double usage { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
