using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer; 

namespace WebApplication.ClientSideData
{
    public class Client_Http_Request
    {
        public string method { get; set; }
        public string path { get; set; }
        public DateTime startTimestamp { get; set; }
        public DateTime endTimestamp { get; set; }
        public long duration { get; set; } //measured in ms

        // Constructor that will take in values that are taken from Http_Request to make
        // a Detailed_Http_Request object.
        public Client_Http_Request(Http_Request h)
        {
            this.method = h.method;
            this.path = h.path; 
            this.startTimestamp = h.timestamp; //will always be the start time when using this constructor
        }

        // When the end timestamp of a Http request becomes available, the corresponding
        // Detailed_Http_Request can be updated using this method. It will also update the duration variable.
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
