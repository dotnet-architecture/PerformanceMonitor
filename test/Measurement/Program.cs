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
            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
            //t.Start();
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
        static void ThreadProc()
        {
            while (true)
            {
                continue;
            }
        }
    }
}
