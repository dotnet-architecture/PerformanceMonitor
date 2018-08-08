﻿using System;
using DataTransfer;

namespace ExceptionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Monitor monitor = new Monitor("New Process", "ExceptionApp", 200, 500);
            monitor.EnableException();
            monitor.Record();
            Console.WriteLine("Goodbye World!");
            Exceptions();
        }
        public static void Exceptions()
        {
            Random random = new Random();
            DateTime timer = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(timer).TotalSeconds >= 1)
                {
                    int rand = random.Next(1, 6);
                    try
                    {
                        if (rand == 1)
                        {
                            throw new DivideByZeroException();
                        }
                        else if (rand == 2)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        else if (rand == 3)
                        {
                            throw new AggregateException();
                        }
                        else if (rand == 4)
                        {
                            throw new ArgumentException();
                        }
                        else if (rand == 5)
                        {
                            throw new ArithmeticException();
                        }
                    }
                    catch (Exception) { }
                    timer = DateTime.Now;
                }
            }
        }
    }
}
