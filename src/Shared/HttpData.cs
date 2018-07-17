using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class HttpData
    {
        public string Type { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public Guid? Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int AppId { get; set; }

        public ApplicationData App { get; set; }
    }
}
