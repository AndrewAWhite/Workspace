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

        public BitArray[] Breed(BitArray p1, BitArray p2)
        {
            var child = new BitArray(p1.Length);
            var rand1 = _random.Next(1000000);
           
            var crossPoint = _random.Next(1, child.Length - 1);
            var crossover = rand1 <= CrossoverRate*1000000;
            return crossover ? CrossOver(p1, p2, crossPoint) : new [] {p1, p2};
        }

        public BitArray[] Breed(BitArray p1, BitArray p2, double mutationRate)
        {
            var children = Breed(p1, p2);
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < children[i].Length; j++)
            {
                var mutate = _random.Next(1000000) < mutationRate*1000000;
                if (mutate) children[i][j] = !children[i][j];
            }
            return children;
        }

        public BitArray[] Breed(Individual p1, Individual p2)
        {
            return Breed(p1.Chromosomes, p2.Chromosomes);
        }

        public BitArray[] Breed(Individual p1, Individual p2, double mutationRate)
        {
            return Breed(p1.Chromosomes, p2.Chromosomes, mutationRate);
        }

        public Population Breed(Population population, Selector selector)
        {
            var chosen = selector.Chosen;
            var childGenes = new BitArray[population.Count];
            for (var i = 0; i < population.Count; i += 2)
            {
                var p1 = chosen[i];
                var p2 = chosen[i + 1];
                var children = Breed(p1, p2);
                childGenes[i] = children[0];
                childGenes[i + 1] = children[1];
            }
            return new Population(childGenes);
        }

        public Population Breed(Population population, Selector selector, double mutationRate)
        {
            var chosen = selector.Chosen;
            var childGenes = new BitArray[population.Count];
            for (var i = 0; i < population.Count; i += 2)
            {
                var p1 = chosen[i];
                var p2 = chosen[i + 1];
                var children = Breed(p1, p2, mutationRate);
                childGenes[i] = children[0];
                childGenes[i + 1] = children[1];
            }
            return new Population(childGenes);
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
        private readonly Func<BitArray, double> _fitnessFunc;
        private readonly Population _population;
        private readonly Random _random = new Random();


        public double AverageFitness { get; }
        public double MaxFitness { get; } = 0;
        public double[] FitnessTable { get;}
        public List<Individual> Chosen { get; } = new List<Individual>();

        public Selector(Population population, Func<BitArray, double> fitnessFunc)
        {
            _population = population;
            _fitnessFunc = fitnessFunc;
            AverageFitness = _population.Average(individual => _fitnessFunc(individual.Chromosomes));
            FitnessTable = new double[_population.Count];
            foreach (var individual in _population)
            {
                var fitness = _fitnessFunc(individual.Chromosomes);
                FitnessTable[individual.Name] = fitness;
                if (fitness > MaxFitness) MaxFitness = fitness;
            }
            
            SpinWheel();
        }

        public void SpinWheel()
        {
            var share = new int[_population.Count];
            foreach (var individual in _population)
            {
                share[individual.Name] = Convert.ToInt32(FitnessTable[individual.Name]/AverageFitness);
            }
            var wheel = new int[share.Sum()];

            var index = 0;
            for (var i = 0; i < share.Length; i++)
            {
                for (var j = 0; j < share[i]; j++)
                {
                    wheel[index] = i;
                    index++;
                }       
            }

            foreach (var i in _population)
            {
                index = _random.Next(wheel.Length);
                Chosen.Add(_population[wheel[index]]);
            }

        }

        public IEnumerator<Individual> GetEnumerator()
        {
            return Chosen.GetEnumerator();
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

        public Population(IEnumerable<BitArray> individualGenes)
        {
            foreach (var c in individualGenes)
            {
                Individuals.Add(new Individual(Count, c));
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

    public class Garden
    {
        public int PopulationCount { get; }
        public Population Population { get; private set; }
        public Func<BitArray, double> FitnessFunc { get; }

        public int ChromosomeLength { get; }
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }

        public int Generation { get; private set; }
        public double MaxFitness { get; private set; }
        public double AverageFitness { get; private set; }

        private readonly Random _random = new Random();
        private readonly Breeder _breeder = new Breeder();

        public Garden(Func<BitArray, double> fitnessFunc, int populationCount, int chromosomeLength)
        {
            FitnessFunc =  fitnessFunc;
            PopulationCount = populationCount;
            ChromosomeLength = chromosomeLength;

            Genesis();
        }

        private void Genesis()
        {
            var individuals = new List<BitArray>();
            for (var i = 0; i < PopulationCount; i++)
            {
                var genes = new BitArray(ChromosomeLength);
                for (var j = 0; j < ChromosomeLength; j++)
                {
                    genes[j] = _random.Next(100) < 50;
                }
                individuals.Add(genes);
            }
            Population = new Population(individuals);
        }

        public void NextGen()
        {
            var selector = new Selector(Population, FitnessFunc);
            MaxFitness = selector.MaxFitness;
            AverageFitness = selector.AverageFitness;
            Population = _breeder.Breed(Population, selector, MutationRate);
            Generation++;
        }
    }
}
