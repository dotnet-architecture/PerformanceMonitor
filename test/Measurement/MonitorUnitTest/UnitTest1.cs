using System;
using Xunit;
using MonitorTest;
using System.Diagnostics;

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
            Debug.Assert(Program.CPUMemTest() >= (ratio - per) || Program.CPUMemTest() <= (ratio - per + per/2));
        }
        
        [Fact]
        public void Test2()
        {
            int sample = Program.getSampleRate();
            Debug.Assert(Program.RequestTest() >= sample - sample / 10 || Program.RequestTest() <= sample + sample / 10);
        }
    }
}
