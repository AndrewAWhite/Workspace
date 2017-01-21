using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workshop.GA;

namespace Workshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(GA.GA.Name);
            Finished();
        }

        public static void Finished()
        {
            Console.WriteLine("Complete.");
            Console.ReadLine();
        }
    }
}
