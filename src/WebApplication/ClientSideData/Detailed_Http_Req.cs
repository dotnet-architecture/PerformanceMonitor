using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer; 

namespace WebApplication.ClientSideData
{
    public class Detailed_Http_Req
    {
        public string method { get; set; }
        public string path { get; set; }
        public Guid id { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public long duration { get; set; } //measured in ms
        public Detailed_Http_Req(string method, string path, Guid id, DateTime start)
        {
            this.method = method;
            this.path = path;
            this.id = id;
            this.startTimestamp = start; 
        }
        public Boolean updateEndTimestamp(DateTime end){
            if (startTimestamp == null && endTimestamp == null)
            {
                return false;
            }

            this.endTimestamp = end; 
            TimeSpan span = endTimestamp.Subtract(startTimestamp);
            duration = span.Milliseconds;
            return true;
        }
    }
}
