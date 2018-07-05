using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Http_Request  // contains type of exception and timestamp
    {
        public String type { get; set; }
        public String method { get; set; }
        public String path { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
