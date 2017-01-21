using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Workshop.GA;
using static Workshop.Collections.LinqExtensions;

namespace Workshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestGa();
            Finished();
        }

        public static void TestGa()
        {
            var fitnessFun = new Func<BitArray, double>(
                b =>
                {
                    var fitness = 0.0;
                    for (var i = 1; i < b.Length; i++)
                    {
                        if (b[i] == b[i - 1]) continue;
                        fitness += 1;
                    }
                    return fitness;
                });
            var environment = new Garden(fitnessFun, 20, 20) {CrossoverRate = 0.7, MutationRate = 0.02};
            while (environment.MaxFitness < 19)
            {
                environment.NextGen();
                Console.WriteLine($"Average: {environment.AverageFitness}\tMax: {environment.MaxFitness}");
            }
        }

        public static void Finished()
        {
            Console.WriteLine("Complete.");
            Console.ReadLine();
        }
    }
}
