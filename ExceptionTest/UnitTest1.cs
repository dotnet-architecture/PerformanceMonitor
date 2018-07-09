using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataTransfer;

namespace ExceptionTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Monitor monitor = new Monitor();
            monitor.Record();
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    int i = 0;
                    int j = 7;
                    try
                    {
                        int k = j / i;
                    }
                    catch (DivideByZeroException)
                    {
                    }
                    timer = DateTime.Now;
                }
            }
        }
    }
}
