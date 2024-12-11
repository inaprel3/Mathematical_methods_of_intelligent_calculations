using System;

public class BiogeographyOptimization
{
    // Number of habitats (individuals)
    static int populationSize = 20;
    // Number of generations
    static int generations = 100;
    // Migration rate
    static double migrationRate = 0.3;
    // Mutation rate
    static double mutationRate = 0.02;

    // Boundaries for x and y values
    static double lowerBound = -2.0;
    static double upperBound = 2.0;

    // Function to optimize
    public static double TestFunction(double x, double y)
    {
        return -2 * x * x - x * y - y * y + 3 * x;
    }

    // Initialize habitats
    public static double[,] InitializePopulation()
    {
        double[,] population = new double[populationSize, 2];
        Random rand = new Random();
        
        for (int i = 0; i < populationSize; i++)
        {
            population[i, 0] = lowerBound + rand.NextDouble() * (upperBound - lowerBound);
            population[i, 1] = lowerBound + rand.NextDouble() * (upperBound - lowerBound);
        }
        
        return population;
    }

    // Fitness evaluation (objective function value)
    public static double[] EvaluateFitness(double[,] population)
    {
        double[] fitness = new double[populationSize];
        
        for (int i = 0; i < populationSize; i++)
        {
            double x = population[i, 0];
            double y = population[i, 1];
            fitness[i] = TestFunction(x, y);
        }
        
        return fitness;
    }

    // Migration
    public static void Migration(double[,] population, double[] fitness)
    {
        Random rand = new Random();
        
        for (int i = 0; i < populationSize; i++)
        {
            if (rand.NextDouble() < migrationRate)
            {
                int target = rand.Next(populationSize);
                population[i, 0] = population[target, 0];
                population[i, 1] = population[target, 1];
            }
        }
    }

    // Mutation
    public static void Mutation(double[,] population)
    {
        Random rand = new Random();
        
        for (int i = 0; i < populationSize; i++)
        {
            if (rand.NextDouble() < mutationRate)
            {
                population[i, 0] = lowerBound + rand.NextDouble() * (upperBound - lowerBound);
                population[i, 1] = lowerBound + rand.NextDouble() * (upperBound - lowerBound);
            }
        }
    }

    public static void Main()
    {
        // Initialize the population
        double[,] population = InitializePopulation();
        double[] fitness = EvaluateFitness(population);

        for (int generation = 0; generation < generations; generation++)
        {
            Migration(population, fitness);
            Mutation(population);
            fitness = EvaluateFitness(population);
        }

        // Find the best solution
        double maxFitness = double.MinValue;
        double bestX = 0;
        double bestY = 0;

        for (int i = 0; i < populationSize; i++)
        {
            if (fitness[i] > maxFitness)
            {
                maxFitness = fitness[i];
                bestX = population[i, 0];
                bestY = population[i, 1];
            }
        }

        Console.WriteLine("Maximum value of the function: " + maxFitness);
        Console.WriteLine("Coordinates of the global optimum point: (" + bestX + "; " + bestY + ")");
    }
}
