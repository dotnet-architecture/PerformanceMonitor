using System;
using System.Collections.Generic;
using System.Text;
using DataTransfer;

namespace MonitorTest
{
    class GCTest
    {
        public static void Test()
        {
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    timer = DateTime.Now;
                }
            }
        }
    }
}
