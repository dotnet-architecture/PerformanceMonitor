using System;
using System.Collections.Generic;
using System.Text;

namespace MonitorTest
{
    class ExceptionTest
    {
        static DateTime timer = DateTime.Now;
        static int i = 0;
        static int j = 2;
        public static void Test()
        {
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    try
                    {
                        int k = j / i;
                    }
                    catch (Exception){}
                    timer = DateTime.Now;
                }
            }
        }
    }
}
