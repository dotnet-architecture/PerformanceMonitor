using System;
using DataTransfer;
using System.Threading;

namespace MonitorTest
{
    public class Program
    {
        static DataTransfer.Monitor monitor = new DataTransfer.Monitor("App");
        static void Main(string[] args)
        {
            monitor.Record();
            //CPUMemTest();
            //GCTest();
            ExceptionTest();
            //ContentionTest();
        }
        public static int getSampleRate()
        {
            return monitor.sampleRate;
        }
        public static int getSendRate()
        {
            return monitor.sendRate;
        }
        public static double RequestTest()
        {
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 20)
                {
                    timer = DateTime.Now;
                    int count = 0;
                    double avg = 0;
                    DateTime newTimer = DateTime.Now;
                    while (DateTime.Now.Subtract(newTimer).TotalMilliseconds <= monitor.sendRate * 4)
                    {
                        if (monitor.getHold() == 1)
                        {
                            avg = (DateTime.Now.Subtract(timer).TotalMilliseconds + avg * count) / (count + 1);
                            count++;
                            timer = DateTime.Now;
                            while (monitor.getHold() == 1) ;
                        }
                    }
                    return avg;
                }
            }
        }
        public static int CPUMemTest()
        {
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 25)
                {
                    timer = DateTime.Now;
                    while (true)
                    {
                        int max = 0;
                        if (DateTime.Now.Subtract(timer).TotalMilliseconds <= monitor.sendRate)
                        {
                            if (monitor.getCPUCount() > max)
                            {
                                max = monitor.getCPUCount();
                            }
                        }
                        return max;
                    }
                }
            }
        }
        public static void GCTest()
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
        public static void ExceptionTest()
        {
            DateTime timer = DateTime.Now;
            int i = 0;
            int j = 2;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    try
                    {
                        int k = j / i;
                    }
                    catch (Exception) { }
                    timer = DateTime.Now;
                }
            }
        }
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
