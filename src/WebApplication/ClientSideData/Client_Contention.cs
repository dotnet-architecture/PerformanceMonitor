using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer;

namespace WebApplication.ClientSideData
{
    public class Client_Contention
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public long Duration { get; set; } //measured in ms

        // Constructor that will take in values that are taken from Http_Request to make
        // a Detailed_Http_Request object.
        public Client_Contention(Contention c)
        {
            this.StartTimestamp = c.timestamp; //will always be the start time when using this constructor
        }

        // When the end timestamp of a Http request becomes available, the corresponding
        // Detailed_Http_Request can be updated using this method. It will also update the duration variable.
        public Boolean updateEndTimestamp(DateTime end)
        {
            if (StartTimestamp == null && EndTimestamp == null)
            {
                return false;
            }

            this.EndTimestamp = end;
            TimeSpan span = EndTimestamp.Subtract(StartTimestamp);
            Duration = span.Milliseconds;
            return true;
        }
    }
}