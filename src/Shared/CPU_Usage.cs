using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class CPU_Usage  // contains the percentage of total CPU usage and DateTime of instant
    {
        public double usage { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
