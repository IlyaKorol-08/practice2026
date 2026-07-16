using Xunit;
using task17;

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsImmediately()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var list = new List<int>();
        server.Enqueue(new TestCommand(() => list.Add(1)));
        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new TestCommand(() => list.Add(2)));
        server.Start();
        server.GetThread().Join(5000);
        Assert.Single(list);
    }

    [Fact]
    public void SoftStop_FinishesQueue()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var list = new List<int>();
        server.Enqueue(new TestCommand(() => list.Add(1)));
        server.Enqueue(new SoftStopCommand(server));
        server.Start();
        server.GetThread().Join(5000);
        Assert.Single(list);
    }

    [Fact]
    public void HardStop_WrongThread_Throws()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var cmd = new HardStopCommand(server);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void SoftStop_WrongThread_Throws()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var cmd = new SoftStopCommand(server);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());
    }

    [Fact]
    public void Exception_IsHandled()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var list = new List<int>();
        server.Enqueue(new TestCommand(() => throw new Exception("err")));
        server.Enqueue(new TestCommand(() => list.Add(1)));
        server.Enqueue(new SoftStopCommand(server));
        server.Start();
        server.GetThread().Join(5000);
        Assert.Single(list);
    }

    [Fact]
    public void LongCommand_RunsInSteps()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var steps = new List<int>();
        server.Enqueue(new LongRunningCommand(5, s, step => steps.Add(step)));
        server.Enqueue(new SoftStopCommand(server));
        server.Start();
        server.GetThread().Join(5000);
        Assert.Equal(5, steps.Count);
    }

    [Fact]
    public void TwoLongCommands_Interleave()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var a = new List<int>();
        var b = new List<int>();
        server.Enqueue(new LongRunningCommand(3, s, step => a.Add(step)));
        server.Enqueue(new LongRunningCommand(3, s, step => b.Add(step)));
        server.Enqueue(new SoftStopCommand(server));
        server.Start();
        server.GetThread().Join(5000);
        Assert.Equal(3, a.Count);
        Assert.Equal(3, b.Count);
    }

    [Fact]
    public void Scheduler_FIFO_Order()
    {
        var s = new RoundRobinScheduler();
        var list = new List<int>();
        s.Add(new TestCommand(() => list.Add(1)));
        s.Add(new TestCommand(() => list.Add(2)));
        s.Select().Execute();
        s.Select().Execute();
        Assert.Equal(new[] { 1, 2 }, list);
    }

    [Fact]
    public void GenerateGraph()
    {
        var s = new RoundRobinScheduler();
        var server = new ServerThread(s);
        var timeline = new List<(string Name, int Step, double Time)>();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        server.Enqueue(new LongRunningCommand(10, s, step =>
            timeline.Add(("C1", step, sw.Elapsed.TotalMilliseconds))));
        server.Enqueue(new LongRunningCommand(10, s, step =>
            timeline.Add(("C2", step, sw.Elapsed.TotalMilliseconds))));
        server.Enqueue(new LongRunningCommand(10, s, step =>
            timeline.Add(("C3", step, sw.Elapsed.TotalMilliseconds))));
        server.Enqueue(new SoftStopCommand(server));

        server.Start();
        server.GetThread().Join(10000);

        var plt = new ScottPlot.Plot();
        var colors = new[] { ScottPlot.Colors.Red, ScottPlot.Colors.Blue, ScottPlot.Colors.Green };
        var names = new[] { "C1", "C2", "C3" };

        for (int i = 0; i < 3; i++)
        {
            var data = timeline.Where(t => t.Name == names[i]).ToList();
            if (data.Any())
            {
                var sc = plt.Add.Scatter(
                    data.Select(d => d.Time).ToArray(),
                    data.Select(d => (double)d.Step).ToArray());
                sc.Color = colors[i];
                sc.LegendText = names[i];
            }
        }

        plt.Title("Round Robin чередование команд");
        plt.XLabel("Время (мс)");
        plt.YLabel("Оставшиеся шаги");
        plt.ShowLegend();
        plt.SavePng("graph.png", 800, 600);

        Assert.True(timeline.Count > 0);
    }
}