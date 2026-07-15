using task17;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsThreadImmediately()
    {
        var server = new ServerThread();
        var commandsExecuted = new List<int>();
        
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(1)));
        server.Enqueue(new HardStopCommand(server));
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(2)));
        
        server.Start();
        server.GetThread().Join(5000);
        
        Assert.Single(commandsExecuted);
        Assert.Equal(1, commandsExecuted[0]);
    }

    [Fact]
    public void SoftStop_ExecutesAllQueuedCommands()
    {
        var server = new ServerThread();
        var commandsExecuted = new List<int>();
        
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(1)));
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(2)));
        server.Enqueue(new SoftStopCommand(server));
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(3)));
        
        server.Start();
        server.GetThread().Join(5000);
        
        Assert.Equal(3, commandsExecuted.Count);
        Assert.Contains(1, commandsExecuted);
        Assert.Contains(2, commandsExecuted);
        Assert.Contains(3, commandsExecuted);
    }

    [Fact]
    public void HardStop_FromAnotherThread_ThrowsException()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        
        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
    }

    [Fact]
    public void SoftStop_FromAnotherThread_ThrowsException()
    {
        var server = new ServerThread();
        var softStop = new SoftStopCommand(server);
        
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }

    [Fact]
    public void ServerThread_HandlesException_ContinuesProcessing()
    {
        var server = new ServerThread();
        var commandsExecuted = new List<int>();
        
        server.Enqueue(new TestCommand(() => throw new Exception("Test exception")));
        server.Enqueue(new TestCommand(() => commandsExecuted.Add(1)));
        server.Enqueue(new SoftStopCommand(server));
        
        server.Start();
        server.GetThread().Join(5000);
        
        Assert.Single(commandsExecuted);
        Assert.Equal(1, commandsExecuted[0]);
    }
}
