
using System.Data.SqlClient;
using Xunit;

namespace UnitTestProject1
{
    public class TestTransfer
    {
        public TestTransfer() { }
        
        [Fact]
        public void TestSQLConnection()
        {
            bool isOpen = false;
            var connection = "Server = 10.0.75.1,1433; Initial Catalog = PerformanceData  ; User Id = sa; Password = Abc12345";

            using (var serCon = new SqlConnection(connection))
            {
                try
                {
                    serCon.Open();
                    isOpen = true;
                }
                catch
                {
                    isOpen = false;
                }
            }
            Assert.True(isOpen);
        }
    }
}
