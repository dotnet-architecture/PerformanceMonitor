using System;
using DataTransfer;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor("Process1", "ConsoleApp");
            monitor.Record();
            Console.WriteLine("Hello World!");
            while (true) ;
        }
    }
}
