using System;
using DataTransfer;

namespace MonitorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor("App");
            monitor.Record();
            CPUMemTest.Test();
            //GCTest.Test();
            //ExceptionTest.Test();

            ContentionTest.Test();
        }
    }
}
