using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class Jit
    {
        public string method { get; set; }
        public DateTime timestamp { get; set; }
        public int AppId { get; set; }

        public Session App { get; set; }
    }
}
