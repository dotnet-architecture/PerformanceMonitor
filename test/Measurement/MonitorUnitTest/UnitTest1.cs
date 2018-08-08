using Xunit;
using MonitorTest;

namespace MonitorUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void SamplingTest()  // makes sure # of samples falls within expected range
        {
            int send = Program.getSendRate();
            int sample = Program.getSampleRate();
            int ratio = send / sample;
            int per;
            if (ratio - 500 / sample < 0)
            {
                per = ratio;
            } else
            {
                per = 500 / sample;
            }
            int ans = Program.UnitTest1();
            Assert.True(ans >= (ratio - per - per/2) && ans <= (ratio - per + per/2));
        }
        
        [Fact]
        public void FrequencyTest()  // makes sure requests are sent as often as expected
        {
            int send = Program.getSendRate();
            double ans = Program.UnitTest2();
            Assert.True(ans >= send - send / 4 && ans <= send + send / 4);
        }
    }
}
