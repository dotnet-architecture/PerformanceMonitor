using Xunit;
using MonitorTest;

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
            int per;
            if (ratio - 2100 / sample < 0)
            {
                per = ratio;
            } else
            {
                per = 2100 / sample;
            }
            int ans = Program.UnitTest1();
            Assert.True(ans >= (ratio - per - per/2) && ans <= (ratio - per + per/2));
        }
        
        [Fact]
        public void Test2()
        {
            int send = Program.getSendRate();
            double ans = Program.UnitTest2();
            Assert.True(ans >= send - send / 5 && ans <= send + send / 5);
        }
    }
}
