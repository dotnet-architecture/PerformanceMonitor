using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class MemData
    {
        public long? Usage { get; set; }
        public DateTime Timestamp { get; set; }
        public int AppId { get; set; }

        public ApplicationData App { get; set; }
    }
}
