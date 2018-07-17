using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class Session
    {
        public Session()
        {
            Contention = new HashSet<Contention>();
            CPU_Usage = new HashSet<CPU_Usage>();
            Exceptions = new HashSet<Exceptions>();
            Gc = new HashSet<GC>();
            Http_Request = new HashSet<Http_Request>();
            Jit = new HashSet<Jit>();
            MemData = new HashSet<Mem_Usage>();
        }

        public string application { get; set; }
        public string process { get; set; }
        public string os { get; set; }
        public int? sampleRate { get; set; }
        public int? sendRate { get; set; }
        public int? processorCount { get; set; }
        public int Id { get; set; }

        public ICollection<Contention> Contention { get; set; }
        public ICollection<CPU_Usage> CPU_Usage { get; set; }
        public ICollection<Exceptions> Exceptions { get; set; }
        public ICollection<GC> Gc { get; set; }
        public ICollection<Http_Request> Http_Request { get; set; }
        public ICollection<Jit> Jit { get; set; }
        public ICollection<Mem_Usage> MemData { get; set; }
    }
}
