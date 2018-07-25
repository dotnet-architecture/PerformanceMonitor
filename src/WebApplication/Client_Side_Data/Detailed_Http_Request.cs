using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ClientSideData
{
    public class Detailed_Http_Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public Guid id { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public long duration { get; set; } //measured in ms

        public Boolean updateDuration(){
            if (startTimestamp != null && endTimestamp != null)
            {
                return false;
            }

            TimeSpan span = endTimestamp.Subtract(startTimestamp);
            duration = span.Milliseconds;
            return true;
        }
}
}
