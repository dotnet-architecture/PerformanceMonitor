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
<<<<<<< HEAD

            CPUMemTest.Test();
=======
            //CPUMemTest.Test();
>>>>>>> upstream/master
            //GCTest.Test();
            //ExceptionTest.Test();
            ContentionTest.Test();
        }
    }
}
