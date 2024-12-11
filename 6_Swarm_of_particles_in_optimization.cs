using System;

class Program
{
    static void Main()
    {
        // Межі допустимих значень змінних
        double lowerBound = -2.0;
        double upperBound = 2.0;

        // Параметри методу рою частинок
        int swarmSize = 30;         // Кількість частинок
        int iterations = 100;       // Кількість ітерацій
        double inertia = 0.5;       // Інерція
        double cognitive = 1.5;     // Ваговий коефіцієнт когнітивної компоненти
        double social = 1.5;        // Ваговий коефіцієнт соціальної компоненти

        // Ініціалізація рою частинок
        Particle[] swarm = new Particle[swarmSize];
        Random random = new Random();
        for (int i = 0; i < swarmSize; i++)
        {
            swarm[i] = new Particle(random, lowerBound, upperBound);
        }

        // Змінні для збереження глобально найкращої позиції
        double[] globalBestPosition = new double[2];
        double globalBestValue = double.MinValue;

        // Основний цикл PSO
        for (int iter = 0; iter < iterations; iter++)
        {
            foreach (var particle in swarm)
            {
                // Оновлюємо швидкість і позицію частинки
                particle.UpdateVelocity(globalBestPosition, inertia, cognitive, social, random);
                particle.UpdatePosition(lowerBound, upperBound);

                // Оновлюємо найкращу позицію частинки
                double fitnessValue = FitnessFunction(particle.Position[0], particle.Position[1]);
                if (fitnessValue > particle.BestValue)
                {
                    particle.BestValue = fitnessValue;
                    particle.BestPosition = (double[])particle.Position.Clone();
                }

                // Оновлюємо глобально найкращу позицію
                if (fitnessValue > globalBestValue)
                {
                    globalBestValue = fitnessValue;
                    globalBestPosition = (double[])particle.Position.Clone();
                }
            }

            // Вивід поточного глобального найкращого значення
            Console.WriteLine($"Ітерація {iter + 1}: Найкраще значення = {globalBestValue:F4} у точці ({globalBestPosition[0]:F4}, {globalBestPosition[1]:F4})");
        }

        // Підсумковий результат
        Console.WriteLine("\nРезультати оптимізації:");
        Console.WriteLine($"Глобальний максимум: {globalBestValue:F4}");
        Console.WriteLine($"Точка глобального максимуму: ({globalBestPosition[0]:F4}, {globalBestPosition[1]:F4})");
    }

    // Фітнес-функція (Параболічна 2 типу)
    static double FitnessFunction(double x, double y)
    {
        return -2 * Math.Pow(x, 2) - x * y - Math.Pow(y, 2) + 3 * x;
    }
}

class Particle
{
    public double[] Position { get; set; }
    public double[] Velocity { get; set; }
    public double[] BestPosition { get; set; }
    public double BestValue { get; set; }

    public Particle(Random random, double lowerBound, double upperBound)
    {
        int dimensions = 2; // Двовимірна задача
        Position = new double[dimensions];
        Velocity = new double[dimensions];
        BestPosition = new double[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            Position[i] = random.NextDouble() * (upperBound - lowerBound) + lowerBound;
            Velocity[i] = random.NextDouble() * (upperBound - lowerBound) / 2.0;
        }

        BestPosition = (double[])Position.Clone();
        BestValue = double.MinValue;
    }

    public void UpdateVelocity(double[] globalBestPosition, double inertia, double cognitive, double social, Random random)
    {
        for (int i = 0; i < Velocity.Length; i++)
        {
            double r1 = random.NextDouble();
            double r2 = random.NextDouble();
            Velocity[i] = inertia * Velocity[i]
                        + cognitive * r1 * (BestPosition[i] - Position[i])
                        + social * r2 * (globalBestPosition[i] - Position[i]);
        }
    }

    public void UpdatePosition(double lowerBound, double upperBound)
    {
        for (int i = 0; i < Position.Length; i++)
        {
            Position[i] += Velocity[i];
            Position[i] = Math.Max(lowerBound, Math.Min(Position[i], upperBound));
        }
    }
}
