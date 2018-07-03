using System;
using System.ComponentModel.DataAnnotations;

namespace PerfMonitor
{
    public class Http_Request  // contains type of exception and timestamp
    {
        public String type { get; set; }
        [Key]
        public String method { get; set; }
        public String path { get; set; }
        public DateTime timestamp { get; set; }
    }
}