using System;

namespace PerfMonitor
{
    public class Metric_List  // class used to aggregate events for transmission via JSON
    {
        public List<CPU_Usage> cpu { get; set; }
        public List<Mem_Usage> mem { get; set; }
    }
}
