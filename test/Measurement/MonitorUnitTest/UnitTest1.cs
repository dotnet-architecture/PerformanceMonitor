using Xunit;
using MonitorTest;

namespace MonitorUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void SamplingTest()  // makes sure # of samples falls within expected range
        {
            double send = (double)Program.getSendRate();
            double sample = (double)Program.getSampleRate();
            double ratio = send / sample;
            int ans = Program.UnitTest1();
            Assert.True(ans >= (ratio - ratio / 4) && ans <= (ratio + ratio / 4));
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
