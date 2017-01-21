using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Workshop.Collections.LinqExtensions;

namespace Workshop.GA
{

    public struct Chromosome
    {
        public BitArray Values { get; private set; }

        public Chromosome(BitArray values)
        {
            Values = values;
        }
    }

    public class Individual
    {
        public BitArray Chromosomes { get; }
        public int Name { get; }

        public Individual(int name, BitArray chromosomes)
        {
            Chromosomes = chromosomes;
            Name = name;
        }
    }

    public class Breeder
    {
        public double CrossoverRate { get; set; } = 0.7;
        private readonly Random _random = new Random();

        public BitArray[] MakeChildren(BitArray p1, BitArray p2)
        {
            var child = new BitArray(p1.Length);
            var rand1 = _random.Next(1000000);
           
            var crossPoint = _random.Next(1, child.Length - 1);
            var crossover = rand1 <= CrossoverRate*1000000;
            return crossover ? CrossOver(p1, p2, crossPoint) : new [] {p1, p2};
        }

        public BitArray[] MakeChildren(BitArray p1, BitArray p2, double mutationRate)
        {
            var children = MakeChildren(p1, p2);
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < children[i].Length; j++)
            {
                var mutate = _random.Next(1000000) < mutationRate*1000000;
                if (mutate) children[i][j] = !children[i][j];
            }
            return children;
        }

        private static BitArray[] CrossOver(BitArray p1, BitArray p2, int crossPoint)
        {
            var children = new[] {new BitArray(p1.Length), new BitArray(p1.Length)};
            for (var i = 0; i < crossPoint; i++)
            {
                children[0][i] = p1[i];
                children[1][i] = p2[i];
            }
            for (var i = crossPoint; i < p1.Length; i++)
            {
                children[0][i] = p2[i];
                children[1][i] = p1[i];
            }
            return children;
        }
    }

    public class Selector : IEnumerable<Individual>
    {
        private readonly Func<BitArray, int> _fitnessFunc;
        private readonly Population _population;
        private readonly Random _random = new Random();

        public double AverageFitness { get; set; }
        public double[] FitnessTable { get;}

        public Selector(Population population, Func<BitArray, int> fitnessFunc)
        {
            _population = population;
            _fitnessFunc = fitnessFunc;
            AverageFitness = _population.Average(individual => _fitnessFunc(individual.Chromosomes));
            FitnessTable = new double[_population.Count];
            foreach (var individual in _population)
            {
                FitnessTable[individual.Name] = _fitnessFunc(individual.Chromosomes);
            }     
        }

        public void RouletteWheel()
        {
            var share = new int[_population.Count];
            foreach (var individual in _population)
            {
                share[individual.Name] = Convert.ToInt32(FitnessTable[individual.Name]/AverageFitness);
            }
            var wheel = new int[share.Sum()];
        }

        public IEnumerator<Individual> GetEnumerator()
        {
            var rand = _random.Next(1000000);
            return _population.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Population : IEnumerable<Individual>
    {
        public List<Individual> Individuals { get; set; } = new List<Individual>();
        public int Count { get; }

        public Individual this[int name] => Individuals[name];

        public Population(IEnumerable<Individual> individuals)
        {
            foreach (var individual in individuals)
            {
                Individuals.Add(individual);
                Count++;
            }
        }

        // Enumerable
        public IEnumerator<Individual> GetEnumerator()
        {
            return Individuals.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
