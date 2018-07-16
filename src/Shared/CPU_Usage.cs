using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class CPU_Usage  // contains the percentage of total CPU usage and DateTime of instant
    {
        public String app { get; set; }
        public String process { get; set; }
        public double usage { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
