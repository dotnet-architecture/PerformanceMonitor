using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Mem_Usage  // contains the number of bytes used by process, and timestamp
    {
        public String app { get; set; }
        public String process { get; set; }
        public long usage { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
