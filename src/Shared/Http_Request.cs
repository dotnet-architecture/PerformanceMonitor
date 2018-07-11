using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Http_Request  // contains type of exception and timestamp
    {
        public String app { get; set; }
        public String process { get; set; }
        public String type { get; set; }
        public String method { get; set; }
        public String path { get; set; }
        public Guid id { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
