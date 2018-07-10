using System;
using System.Threading;

namespace MonitorTest
{
    class ContentionTest
    {
        static int valueType;
        static object valueTypeLock = new object();
        public static void Test()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start();
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    lock (valueTypeLock)
                    {
                        while (DateTime.Now.Subtract(timer).TotalSeconds < 2)
                        {
                            valueType = 0;
                        }
                    }
                    timer = DateTime.Now;
                }
            }
        }
        public static void ThreadMethod()
        {
            while (true)
            {
                lock (valueTypeLock)
                {
                    while (true)
                    {
                        valueType = 1;
                    }
                }
            }
        }
    }
}
