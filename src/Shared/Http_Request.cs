using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Http_Request  // contains type of exception and timestamp
    {
        public string type { get; set; }
        public string method { get; set; }
        public string path { get; set; }
        public Guid activityID { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }
        public int id { get; set; }

        public Session App { get; set; }
    }
}
