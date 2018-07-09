using System;
using DataTransfer;

namespace MonitorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor();
            monitor.Record();
            //GCTest.Test();
            //ExceptionTest.Test();
            //ContentionTest.Test();
        }
    }
}
