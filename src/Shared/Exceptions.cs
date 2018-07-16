using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Exceptions  // contains type of exception and timestamp
    {
        public String app { get; set; }
        public String process { get; set; }
        public String type { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
