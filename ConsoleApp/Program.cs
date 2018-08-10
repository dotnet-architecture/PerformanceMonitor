using System;
using DataTransfer;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize instance of Monitor class with desired process and application names
            Monitor monitor = new Monitor("world", "hit");

            // begin metric recording by Monitor instance
            // by default, this tracks just CPU and memory usage
            monitor.Record();

            Console.WriteLine("Hello World!");

            // loop forever
            while (true) ;
        }
    }
}
