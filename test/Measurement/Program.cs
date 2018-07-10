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

            while (true)
            {
            }
            //GCTest.Test();
            //ExceptionTest.Test();
        }
    }
}
