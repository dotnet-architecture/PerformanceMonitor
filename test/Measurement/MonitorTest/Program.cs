using System;
using DataTransfer;
using System.Threading;

namespace MonitorTest
{
    public class Program
    {
        static DataTransfer.Monitor monitor = new DataTransfer.Monitor("Process", "App1");
        static void Main(string[] args)
        {
            CPUMemTest();
            GCTest();
            ExceptionTest();
            ContentionTest();
        }
        public static int getSampleRate()
        {
            return monitor.sampleRate;
        }
        public static int getSendRate()
        {
            return monitor.sendRate;
        }
        public static void CPUMemTest()
        {
            monitor.Record();
            while (true) ;
        }
        public static double UnitTest2()
        {
            monitor.Record();
            DateTime timer = DateTime.Now;
            while (DateTime.Now.Subtract(timer).TotalMilliseconds <= monitor.sendRate * 4) ;
            timer = DateTime.Now;
            double total = 0.0;
            DateTime newTimer = DateTime.Now;
            while (DateTime.Now.Subtract(newTimer).TotalMilliseconds <= monitor.sendRate * 4)
            {
                if (monitor.getHold() == 1)
                {
                    total += DateTime.Now.Subtract(timer).TotalMilliseconds;
                    timer = DateTime.Now;
                    while (monitor.getHold() == 1) ;
                }
            }
            double avg = total / 4;
            return avg;
        }
        public static int UnitTest1()
        {
            DateTime timer = DateTime.Now;
            while (DateTime.Now.Subtract(timer).TotalMilliseconds <= monitor.sendRate * 4) ;
            timer = DateTime.Now;
            int max = 0;
            while (DateTime.Now.Subtract(timer).TotalMilliseconds <= monitor.sendRate)
            {
                if (monitor.getCPUCount() > max)
                {
                    max = monitor.getCPUCount();
                }
            }
            return max;
        }
        public static void GCTest()
        {
            monitor.Record();
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
            monitor.Record();
            Random random = new Random();
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    int rand = random.Next(1, 6);
                    try
                    {
                        if (rand == 1)
                        {
                            throw new DivideByZeroException();
                        }
                        else if (rand == 2)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        else if (rand == 3)
                        {
                            throw new AggregateException();
                        }
                        else if (rand == 4)
                        {
                            throw new ArgumentException();
                        }
                        else if (rand == 5)
                        {
                            throw new ArithmeticException();
                        }
                    }
                    catch (Exception) { }
                    timer = DateTime.Now;
                }
            }
        }

        static int valueType;
        static object valueTypeLock = new object();
        public static void ContentionTest()
        {
            monitor.Record();
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
