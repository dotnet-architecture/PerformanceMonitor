using System;
using DataTransfer.Controllers;
using System.Data.SqlClient;

namespace UnitTestProject1
{   
    public class TestTransfer
    {
        public TestTransfer() { }

        public bool TestSQLConnection()
        {

            var connection = "Server = 10.0.75.1,1433; Initial Catalog = PerformanceData  ; User Id = sa; Password = JBKmichigan20";
            using (var serCon = new SqlConnection(connection))
            {
                try
                {
                    serCon.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
