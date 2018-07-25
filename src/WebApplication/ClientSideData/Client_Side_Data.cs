using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ClientSideData
{
    public class Client_Side_Data
    {
        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public double Duration { get; set; } //measured in ms

        // When the end timestamp of a metric becomes available, the corresponding
        // Client side metric can be updated using this method. It will also update the duration variable.
        public Boolean updateEndTimestamp(DateTime end)
        {
            if (StartTimestamp == null && EndTimestamp == null)
            {
                return false;
            }

            this.EndTimestamp = end;
            TimeSpan span = EndTimestamp.Subtract(StartTimestamp);
            Duration = span.TotalMilliseconds;
            return true;
        }
    }
}
