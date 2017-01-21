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
            var random = new Random();
            var individuals = new List<Individual>();
            for (var i = 0; i < 200; i++)
            {

                var chromosomes = new BitArray(20);
                for (var j = 0; j < 20; j++)
                {
                    var bit = random.Next(100) < 50;
                    chromosomes[j] = bit;
                }
                individuals.Add(new Individual(i, chromosomes));
            }
            var population = new Population(individuals);
            var fitnessFunc = new Func<BitArray, int>(c => c.Count(b => b));
            var selector = new Selector(population, fitnessFunc);
            selector.RouletteWheel();
        }

        public static void Finished()
        {
            Console.WriteLine("Complete.");
            Console.ReadLine();
        }
    }
}
