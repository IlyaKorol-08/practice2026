using System.Diagnostics;
using task14;

double SIN(double x) => Math.Sin(x);
double a = -100, b = 100;
int runs = 10;

// Берём шаг, который даёт достаточную нагрузку
double step = 1e-4;

Console.WriteLine($"Исследование с шагом {step}\n");

// Однопоточная версия
Console.WriteLine("Однопоточная версия");
var swSingle = Stopwatch.StartNew();
for (int i = 0; i < runs; i++)
{
    DefiniteIntegral.SolveSingleThread(a, b, SIN, step);
}
swSingle.Stop();
double singleTime = swSingle.ElapsedMilliseconds / (double)runs;
Console.WriteLine($"Время: {singleTime:F2} мс\n");

// Подбор оптимального числа потоков
Console.WriteLine("Подбор оптимального числа потоков");
int[] threadCounts = { 1, 2, 4, 6, 8, 10, 12, 14, 16, 20, 24, 28, 32 };
var results = new List<(int threads, double time)>();

foreach (int threads in threadCounts)
{
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < runs; i++)
    {
        DefiniteIntegral.Solve(a, b, SIN, step, threads);
    }
    sw.Stop();
    double avgTime = sw.ElapsedMilliseconds / (double)runs;
    results.Add((threads, avgTime));
    double speedup = (singleTime - avgTime) / singleTime * 100;
    Console.WriteLine($"Потоков: {threads,3} | Время: {avgTime:F2} мс | Ускорение: {speedup:F1}%");
}

var bestMulti = results.OrderBy(r => r.time).First();
double bestSpeedup = (singleTime - bestMulti.time) / singleTime * 100;

Console.WriteLine($"\nРезультаты");
Console.WriteLine($"Однопоточная: {singleTime:F2} мс");
Console.WriteLine($"Лучшая многопоточная: {bestMulti.threads} потоков, {bestMulti.time:F2} мс");
Console.WriteLine($"Ускорение: {bestSpeedup:F1}%");

// График
var plt = new ScottPlot.Plot();
double[] threadAxis = results.Select(r => (double)r.threads).ToArray();
double[] timeAxis = results.Select(r => r.time).ToArray();

plt.Add.Scatter(threadAxis, timeAxis);

// Красная горизонтальная линия — однопоточное время
var line = plt.Add.HorizontalLine(singleTime);
line.Color = ScottPlot.Colors.Red;

plt.Title($"Интеграл sin(x) на [-100, 100], шаг {step}");
plt.XLabel("Количество потоков");
plt.YLabel("Время (мс)");
plt.SavePng("performance_graph.png", 800, 600);
Console.WriteLine("График сохранён в performance_graph.png");

// Отчёт
string report = $@"
Результаты исследования:
Функция: sin(x)
Отрезок: [{a}, {b}]
Шаг: {step}
Точность: 1e-4
Количество запусков для усреднения: {runs}
Оптимальное число потоков: {bestMulti.threads}
Время однопоточной версии: {singleTime:F2} мс
Время многопоточной версии: {bestMulti.time:F2} мс
Ускорение: {bestSpeedup:F1}%
";

File.WriteAllText("research_results.txt", report);
Console.WriteLine(report);