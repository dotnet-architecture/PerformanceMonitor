using System;

namespace DataTransfer
{
    public class Session
    {
        public String app { get; set; }
        public String process { get; set; }
        public int sampleRate { get; set; }
        public int sendRate { get; set; }
        public int processorCount { get; set; }
        public String os { get; set; }
    }
}
