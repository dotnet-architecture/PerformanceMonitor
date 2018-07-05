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
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
            t.Start();
            while (true)
            {
                continue;
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
