﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransfer
{
    public class Contention  // contains the percentage of total CPU usage and DateTime of instant
    {
        public String app { get; set; }
        public String process { get; set; }
        public String type { get; set; }
        [Key]
        public DateTime timestamp { get; set; }
    }
}
