using System.Net.Http;

namespace TestingHTTP
{
    class Program
    {
        
        static void Main(string[] args)
        {
            testProgram p = new testProgram();
            p.pushToServerPlease();
        }
        
        
    }
    class testProgram
    {
        public void pushToServerPlease()
        {
            HttpClient client = new HttpClient();
            string data = "{\"cpu\":[{\"usage\":25.719369797012792,\"timestamp\":\"2018-06-21T14:48:09.3598959-07:00\"},{\"usage\":25.075177065895453,\"timestamp\":\"2018-06-21T14:48:10.3335306-07:00\"},{\"usage\":25.107041360134797,\"timestamp\":\"2018-06-21T14:48:11.3292672-07:00\"},{\"usage\":24.996935375722938,\"timestamp\":\"2018-06-21T14:48:12.3293898-07:00\"},{\"usage\":24.99340673930217,\"timestamp\":\"2018-06-21T14:48:13.3296536-07:00\"}],\"mem\":[{\"usage\":19595264,\"timestamp\":\"2018-06-21T14:48:09.3884843-07:00\"},{\"usage\":19832832,\"timestamp\":\"2018-06-21T14:48:10.3360711-07:00\"},{\"usage\":19832832,\"timestamp\":\"2018-06-21T14:48:11.3322844-07:00\"},{\"usage\":19841024,\"timestamp\":\"2018-06-21T14:48:12.3320021-07:00\"},{\"usage\":20008960,\"timestamp\":\"2018-06-21T14:48:13.3346999-07:00\"}]}";
            StringContent stringContent = new StringContent(data);
            var response = client.PostAsync("https://localhost:54022/api/v1/CCPU/PUJSON/", stringContent);
            var holder = response.Result;
        }
    }
}
