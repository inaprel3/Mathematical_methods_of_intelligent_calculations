using System;

public class Individual {
    public double X { get; set; }
    public double Y { get; set; }
    public double Fitness { get; set; }
    private static Random rand = new Random();

    public Individual() {
        X = rand.NextDouble() * 4 - 2; // [-2; 2]
        Y = rand.NextDouble() * 4 - 2; // [-2; 2]
        Fitness = 0;
    }

    public void CalculateFitness() {
        Fitness = -2 * Math.Pow(X, 2) - X * Y - Math.Pow(Y, 2) + 3 * X + 5;
    }
}

public class GeneticAlgorithm {
    private int populationSize;
    private double mutationRate;
    private int generations;
    private Individual[] population;
    private Random rand = new Random();

    public GeneticAlgorithm(int populationSize, double mutationRate, int generations) {
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.generations = generations;
        population = new Individual[populationSize];

        for (int i = 0; i < populationSize; i++) {
            population[i] = new Individual();
        }
    }

    public Individual Run() {
        Individual bestSolution = null;

        for (int generation = 0; generation < generations; generation++) {
            foreach (var individual in population) {
                individual.CalculateFitness();
            }
            population = CreateNewPopulation();
            bestSolution = GetBestIndividual();

            PrintGenerationInfo(generation, bestSolution);
        }
        return bestSolution;
    }

    private void PrintGenerationInfo(int generation, Individual best) {
        string fitnessOutput = $"{best.Fitness:F4}";

        Console.WriteLine($"Generation {generation}: Best fitness = {fitnessOutput}, X = {best.X:F4}, Y = {best.Y:F4}");
    }

    private Individual[] CreateNewPopulation() {
        Individual[] newPopulation = new Individual[populationSize];

        for (int i = 0; i < populationSize; i++) {
            var parent1 = TournamentSelection();
            var parent2 = TournamentSelection();
            var child = Crossover(parent1, parent2);
            newPopulation[i] = child;
        }

        foreach (var individual in newPopulation) {
            Mutate(individual);
        }
        return newPopulation;
    }

    private Individual TournamentSelection() {
        int tournamentSize = 5;
        Individual best = null;

        for (int i = 0; i < tournamentSize; i++) {
            var individual = population[rand.Next(populationSize)];
            if (best == null || individual.Fitness > best.Fitness) {
                best = individual;
            }
        }
        return best;
    }

    private Individual Crossover(Individual parent1, Individual parent2) {
        double alpha = rand.NextDouble();
        double x = alpha * parent1.X + (1 - alpha) * parent2.X;
        double y = alpha * parent1.Y + (1 - alpha) * parent2.Y;

        return new Individual { X = x, Y = y };
    }

    private void Mutate(Individual individual) {
        if (rand.NextDouble() < mutationRate) {
            individual.X += (rand.NextDouble() * 0.2 - 0.1);
            individual.X = Math.Clamp(individual.X, -2, 2);

            individual.Y += (rand.NextDouble() * 0.2 - 0.1);
            individual.Y = Math.Clamp(individual.Y, -2, 2);
        }
    }

    public Individual GetBestIndividual() {
        Individual best = population[0];

        foreach (var individual in population) {
            if (individual.Fitness > best.Fitness) {
                best = individual;
            }
        }
        return best;
    }
}

public class Program {
    public static void Main(string[] args) {
        var ga = new GeneticAlgorithm(
            populationSize: 100,
            mutationRate: 0.1,
            generations: 50
        );

        Individual bestSolution = ga.Run();

        Console.WriteLine($"\nBest solution: X = {bestSolution.X:F4}, Y = {bestSolution.Y:F4}, Fitness = {bestSolution.Fitness:F4}");
    }
}
