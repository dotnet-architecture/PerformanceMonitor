using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class GcData
    {
        public string Type { get; set; }
        public int? Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int AppId { get; set; }

        public ApplicationData App { get; set; }
    }
}
