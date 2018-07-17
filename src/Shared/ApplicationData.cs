using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public partial class ApplicationData
    {
        public ApplicationData()
        {
            ContentionData = new HashSet<ContentionData>();
            CpuData = new HashSet<CpuData>();
            ExceptionData = new HashSet<ExceptionData>();
            GcData = new HashSet<GcData>();
            HttpData = new HashSet<HttpData>();
            JitData = new HashSet<JitData>();
            MemData = new HashSet<MemData>();
        }

        public string Application { get; set; }
        public string Process { get; set; }
        public int Id { get; set; }

        public ICollection<ContentionData> ContentionData { get; set; }
        public ICollection<CpuData> CpuData { get; set; }
        public ICollection<ExceptionData> ExceptionData { get; set; }
        public ICollection<GcData> GcData { get; set; }
        public ICollection<HttpData> HttpData { get; set; }
        public ICollection<JitData> JitData { get; set; }
        public ICollection<MemData> MemData { get; set; }
    }
}
