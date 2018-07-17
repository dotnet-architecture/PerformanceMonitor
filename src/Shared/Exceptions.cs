using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Exceptions  // contains type of exception and timestamp
    {
        public string type { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
