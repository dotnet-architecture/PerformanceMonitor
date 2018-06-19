using System;

namespace PerfMonitor
{
    public class CPU_Usage  // contains the percentage of total CPU usage and DateTime of instant
    {
        public double usage { get; set; }
        public DateTime timestamp { get; set; }
    }
}
