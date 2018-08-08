using System;
using DataTransfer;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //put comments
            Monitor monitor = new Monitor("Process1", "ConsoleApp", 200, 500);
            monitor.Record();
            Console.WriteLine("Hello World!");
            while (true) ;
        }
    }
}
