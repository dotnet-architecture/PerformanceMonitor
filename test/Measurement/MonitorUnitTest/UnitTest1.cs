using System;
using Xunit;
using MonitorTest;
using System.Diagnostics;
//[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MonitorUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            int send = Program.getSendRate();
            int sample = Program.getSampleRate();
            int ratio = send / sample;
            int per = 2000 / sample;
            int ans = Program.UnitTest1();
            Assert.True(ans >= (ratio - per) && ans <= (ratio - per + per/2));
        }
        
        [Fact]
        public void Test2()
        {
            int send = Program.getSendRate();
            double ans = Program.UnitTest2();
            Assert.True(ans >= send - send / 3 && ans <= send + send / 3);
        }
    }
}
