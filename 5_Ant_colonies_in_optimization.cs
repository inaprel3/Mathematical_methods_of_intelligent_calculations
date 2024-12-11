using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        // Міста та відстані між ними
        string[] cities = { "Київ", "Львів", "Одеса", "Харків", "Дніпро", "Запоріжжя", "Чернівці", "Ужгород", "Полтава", "Вінниця" };
        int cityCount = cities.Length;
        Random random = new Random();

        // Генеруємо матрицю відстаней
        int[,] distances = GenerateRandomDistanceMatrix(cityCount, random);
        PrintDistanceMatrix(distances, cities);

        // Параметри алгоритму
        int antCount = 10;
        int iterations = 100;
        double alpha = 1.0;  // Вплив феромону
        double beta = 5.0;   // Вплив евристики (інверсія відстані)
        double evaporationRate = 0.5; // Швидкість випаровування феромону
        double initialPheromone = 1.0;

        // Запуск алгоритму мурашиних колоній
        var aco = new AntColonyOptimization(cityCount, distances, alpha, beta, evaporationRate, initialPheromone);
        var result = aco.Run(antCount, iterations);

        // Вивід результатів
        Console.WriteLine("\nНайкоротший маршрут:");
        foreach (var city in result.BestRoute)
        {
            Console.Write($"{cities[city]} -> ");
        }
        Console.WriteLine(cities[result.BestRoute[0]]);
        Console.WriteLine($"Довжина маршруту: {result.BestDistance}");
    }

    // Генерація випадкової матриці відстаней
    static int[,] GenerateRandomDistanceMatrix(int size, Random random)
    {
        int[,] matrix = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                matrix[i, j] = random.Next(10, 100); // Відстані між 10 та 100
                matrix[j, i] = matrix[i, j];
            }
        }
        return matrix;
    }

    // Вивід матриці відстаней
    static void PrintDistanceMatrix(int[,] distances, string[] cities)
    {
        Console.WriteLine("Матриця відстаней:");
        Console.Write("       ");
        foreach (var city in cities) Console.Write($"{city,10}");
        Console.WriteLine();
        for (int i = 0; i < distances.GetLength(0); i++)
        {
            Console.Write($"{cities[i],7}");
            for (int j = 0; j < distances.GetLength(1); j++)
            {
                Console.Write($"{distances[i, j],10}");
            }
            Console.WriteLine();
        }
    }
}

class AntColonyOptimization
{
    private int cityCount;
    private int[,] distances;
    private double alpha;
    private double beta;
    private double evaporationRate;
    private double[,] pheromones;

    public AntColonyOptimization(int cityCount, int[,] distances, double alpha, double beta, double evaporationRate, double initialPheromone)
    {
        this.cityCount = cityCount;
        this.distances = distances;
        this.alpha = alpha;
        this.beta = beta;
        this.evaporationRate = evaporationRate;
        pheromones = new double[cityCount, cityCount];
        for (int i = 0; i < cityCount; i++)
        {
            for (int j = 0; j < cityCount; j++)
            {
                pheromones[i, j] = initialPheromone;
            }
        }
    }

    public (List<int> BestRoute, int BestDistance) Run(int antCount, int iterations)
    {
        List<int> bestRoute = null;
        int bestDistance = int.MaxValue;

        for (int iter = 0; iter < iterations; iter++)
        {
            List<List<int>> allRoutes = new List<List<int>>();
            List<int> distancesList = new List<int>();

            for (int ant = 0; ant < antCount; ant++)
            {
                List<int> route = GenerateRoute();
                int distance = CalculateDistance(route);
                allRoutes.Add(route);
                distancesList.Add(distance);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestRoute = new List<int>(route);
                }
            }

            UpdatePheromones(allRoutes, distancesList);
        }

        return (bestRoute, bestDistance);
    }

    private List<int> GenerateRoute()
    {
        List<int> route = new List<int>();
        HashSet<int> visited = new HashSet<int>();
        Random random = new Random();

        int currentCity = random.Next(cityCount);
        route.Add(currentCity);
        visited.Add(currentCity);

        while (route.Count < cityCount)
        {
            int nextCity = SelectNextCity(currentCity, visited);
            route.Add(nextCity);
            visited.Add(nextCity);
            currentCity = nextCity;
        }

        return route;
    }

    private int SelectNextCity(int currentCity, HashSet<int> visited)
    {
        double[] probabilities = new double[cityCount];
        double sum = 0;

        for (int i = 0; i < cityCount; i++)
        {
            if (!visited.Contains(i))
            {
                probabilities[i] = Math.Pow(pheromones[currentCity, i], alpha) * Math.Pow(1.0 / distances[currentCity, i], beta);
                sum += probabilities[i];
            }
        }

        double rand = new Random().NextDouble() * sum;
        for (int i = 0; i < cityCount; i++)
        {
            if (!visited.Contains(i))
            {
                rand -= probabilities[i];
                if (rand <= 0) return i;
            }
        }

        return -1; // Це не повинно статись
    }

    private int CalculateDistance(List<int> route)
    {
        int distance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            distance += distances[route[i], route[i + 1]];
        }
        distance += distances[route[route.Count - 1], route[0]]; // Замикання маршруту
        return distance;
    }

    private void UpdatePheromones(List<List<int>> allRoutes, List<int> distancesList)
    {
        for (int i = 0; i < cityCount; i++)
        {
            for (int j = 0; j < cityCount; j++)
            {
                pheromones[i, j] *= (1.0 - evaporationRate);
            }
        }

        for (int ant = 0; ant < allRoutes.Count; ant++)
        {
            List<int> route = allRoutes[ant];
            int routeDistance = distancesList[ant];
            double pheromoneContribution = 1.0 / routeDistance;

            for (int i = 0; i < route.Count - 1; i++)
            {
                pheromones[route[i], route[i + 1]] += pheromoneContribution;
                pheromones[route[i + 1], route[i]] += pheromoneContribution;
            }

            pheromones[route[route.Count - 1], route[0]] += pheromoneContribution;
            pheromones[route[0], route[route.Count - 1]] += pheromoneContribution;
        }
    }
}
