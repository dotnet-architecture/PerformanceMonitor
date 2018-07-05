using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class GC  // contains the percentage of total CPU usage and DateTime of instant
    {
        public String type { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
