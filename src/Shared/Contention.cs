using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class Contention
    {
        public string type { get; set; }
        public DateTime timestamp { get; set; }
        public Guid id { get; set; }

        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
