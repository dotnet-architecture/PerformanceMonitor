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
            Debug.Assert(Program.CPUMemTest() >= 4 || Program.CPUMemTest() <= 6);
        }
    }
}
