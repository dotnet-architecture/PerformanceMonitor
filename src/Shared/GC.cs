using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class GC
    {
        public string type { get; set; }
        public int id { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
