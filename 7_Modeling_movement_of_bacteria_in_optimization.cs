using System;

class Program
{
    static void Main()
    {
        // Bounds for variables
        double lowerBound = -2.0;
        double upperBound = 2.0;

        // BFO parameters
        int numBacteria = 30;       // Number of bacteria
        int numIterations = 50;    // Number of iterations
        int chemotaxisSteps = 10;  // Number of chemotaxis steps
        double stepSize = 0.1;     // Step size for bacteria
        double eliminationProbability = 0.1; // Elimination probability

        Random random = new Random();

        // Initialize bacteria
        Bacterium[] bacteria = new Bacterium[numBacteria];
        for (int i = 0; i < numBacteria; i++)
        {
            bacteria[i] = new Bacterium(random, lowerBound, upperBound);
        }

        double[] globalBestPosition = new double[2];
        double globalBestValue = double.MinValue;

        // Main BFO loop
        for (int iter = 0; iter < numIterations; iter++)
        {
            foreach (var bacterium in bacteria)
            {
                for (int step = 0; step < chemotaxisSteps; step++)
                {
                    // Update bacterium position
                    bacterium.Chemotaxis(stepSize, random, lowerBound, upperBound);

                    // Evaluate fitness at new position
                    double fitnessValue = FitnessFunction(bacterium.Position[0], bacterium.Position[1]);
                    if (fitnessValue > bacterium.BestValue)
                    {
                        bacterium.BestValue = fitnessValue;
                        bacterium.BestPosition = (double[])bacterium.Position.Clone();
                    }

                    // Update global best
                    if (fitnessValue > globalBestValue)
                    {
                        globalBestValue = fitnessValue;
                        globalBestPosition = (double[])bacterium.Position.Clone();
                    }
                }
            }

            // Elimination and dispersion
            for (int i = 0; i < numBacteria; i++)
            {
                if (random.NextDouble() < eliminationProbability)
                {
                    bacteria[i].RandomizePosition(random, lowerBound, upperBound);
                }
            }

            // Output results after each iteration
            Console.WriteLine($"Iteration {iter + 1}: Best Value = {globalBestValue:F4} at ({globalBestPosition[0]:F4}, {globalBestPosition[1]:F4})");
        }

        // Final results
        Console.WriteLine("\nOptimization Results:");
        Console.WriteLine($"Global Maximum: {globalBestValue:F4}");
        Console.WriteLine($"Global Maximum Point: ({globalBestPosition[0]:F4}, {globalBestPosition[1]:F4})");
    }

    // Fitness function (Parabolic Type 2)
    static double FitnessFunction(double x, double y)
    {
        return -2 * Math.Pow(x, 2) - x * y - Math.Pow(y, 2) + 3 * x;
    }
}

class Bacterium
{
    public double[] Position { get; set; }
    public double[] BestPosition { get; set; }
    public double BestValue { get; set; }

    public Bacterium(Random random, double lowerBound, double upperBound)
    {
        int dimensions = 2; // Two-dimensional problem
        Position = new double[dimensions];
        BestPosition = new double[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            Position[i] = random.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }

        BestPosition = (double[])Position.Clone();
        BestValue = double.MinValue;
    }

    public void Chemotaxis(double stepSize, Random random, double lowerBound, double upperBound)
    {
        for (int i = 0; i < Position.Length; i++)
        {
            Position[i] += stepSize * (random.NextDouble() * 2 - 1); // Random movement
            Position[i] = Math.Max(lowerBound, Math.Min(Position[i], upperBound)); // Boundary check
        }
    }

    public void RandomizePosition(Random random, double lowerBound, double upperBound)
    {
        for (int i = 0; i < Position.Length; i++)
        {
            Position[i] = random.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }
    }
}
