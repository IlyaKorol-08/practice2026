namespace task14;

public class DefiniteIntegral
{
    // Многопоточная версия
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double totalResult = 0.0;
        object lockObject = new object();
        double segmentLength = (b - a) / threadsNumber;
        
        Thread[] threads = new Thread[threadsNumber];
        Barrier barrier = new Barrier(threadsNumber + 1);

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;
            double localA = a + segmentLength * threadIndex;
            double localB = localA + segmentLength;

            threads[i] = new Thread(() =>
            {
                double localResult = CalculateSegment(localA, localB, function, step);
                
                lock (lockObject)
                {
                    totalResult += localResult;
                }
                
                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        barrier.SignalAndWait();
        return totalResult;
    }

    // Однопоточная версия (без потоков вообще)
    public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
    {
        return CalculateSegment(a, b, function, step);
    }

    private static double CalculateSegment(double a, double b, Func<double, double> f, double step)
    {
        double result = 0.0;
        int stepsCount = (int)((b - a) / step);

        for (int i = 0; i < stepsCount; i++)
        {
            double x1 = a + i * step;
            double x2 = x1 + step;
            result += (f(x1) + f(x2)) * step / 2.0;
        }

        return result;
    }
}