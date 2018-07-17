using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Mem_Usage  // contains the number of bytes used by process, and timestamp
    {
        public long usage { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
