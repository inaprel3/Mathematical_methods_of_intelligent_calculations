using System;
using System.Collections.Generic;

public class AISOptimization
{
    private static readonly Random random = new Random();
    private int populationSize = 100; // Кількість антитіл
    private int maxGenerations = 1000; // Максимальна кількість поколінь
    private double mutationRate = 0.1; // Шанс мутації

    // Функція для оптимізації
    private double ObjectiveFunction(double x, double y)
    {
        return -2 * Math.Pow(x, 2) - x * y - Math.Pow(y, 2) + 3 * x;
    }

    // Метод для генерації початкової популяції
    private List<(double x, double y)> InitializePopulation()
    {
        var population = new List<(double, double)>();
        for (int i = 0; i < populationSize; i++)
        {
            double x = random.NextDouble() * 4 - 2; // x ∈ [-2, 2]
            double y = random.NextDouble() * 4 - 2; // y ∈ [-2, 2]
            population.Add((x, y));
        }
        return population;
    }

    // Оцінка популяції
    private List<(double x, double y, double fitness)> EvaluatePopulation(List<(double x, double y)> population)
    {
        var evaluatedPopulation = new List<(double x, double y, double fitness)>();
        foreach (var individual in population)
        {
            double fitness = ObjectiveFunction(individual.x, individual.y);
            evaluatedPopulation.Add((individual.x, individual.y, fitness));
        }
        return evaluatedPopulation;
    }

    // Метод мутації
    private (double x, double y) Mutate((double x, double y) antibody)
    {
        double x = antibody.x + (random.NextDouble() - 0.5) * mutationRate;
        double y = antibody.y + (random.NextDouble() - 0.5) * mutationRate;
        
        // Обмеження значень в межах [-2, 2]
        x = Math.Max(-2, Math.Min(2, x));
        y = Math.Max(-2, Math.Min(2, y));
        
        return (x, y);
    }

    // Основний метод для виконання оптимізації
    public (double x, double y, double maxFitness) Run()
    {
        var population = InitializePopulation();
        double bestFitness = double.MinValue;
        (double x, double y) bestSolution = (0, 0);

        for (int generation = 0; generation < maxGenerations; generation++)
        {
            var evaluatedPopulation = EvaluatePopulation(population);

            foreach (var antibody in evaluatedPopulation)
            {
                if (antibody.fitness > bestFitness)
                {
                    bestFitness = antibody.fitness;
                    bestSolution = (antibody.x, antibody.y);
                }
            }

            // Мутація популяції
            var newPopulation = new List<(double x, double y)>();
            foreach (var antibody in population)
            {
                var mutatedAntibody = Mutate(antibody);
                newPopulation.Add(mutatedAntibody);
            }
            population = newPopulation;
        }

        return (bestSolution.x, bestSolution.y, bestFitness);
    }
}

public class Program
{
    public static void Main()
    {
        AISOptimization ais = new AISOptimization();
        var result = ais.Run();
        Console.WriteLine($"Оптимальне значення: f({result.x:F4}, {result.y:F4}) = {result.maxFitness:F4}");
    }
}
