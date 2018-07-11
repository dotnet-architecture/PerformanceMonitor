using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Jit  // contains the percentage of total CPU usage and DateTime of instant
    {
        public String app { get; set; }
        public String process { get; set; }
        public String method { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
