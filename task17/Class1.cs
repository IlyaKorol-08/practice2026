using System.Collections.Concurrent;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private volatile bool _hardStop = false;
    private volatile bool _softStop = false;

    public ServerThread()
    {
        _thread = new Thread(Run)
        {
            IsBackground = true
        };
    }

    public void Start()
    {
        _thread.Start();
    }

    public Thread GetThread()
    {
        return _thread;
    }

    public void Enqueue(ICommand command)
    {
        _queue.Add(command);
    }

    public void HardStop()
    {
        _hardStop = true;
    }

    public void SoftStop()
    {
        _softStop = true;
    }

    private void Run()
    {
        while (!_hardStop)
        {
            if (_softStop && _queue.Count == 0)
                break;

            try
            {
                if (_queue.TryTake(out var command, TimeSpan.FromMilliseconds(100)))
                {
                    command.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in command: {ex.Message}");
            }
        }
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public HardStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _targetThread.GetThread())
            throw new InvalidOperationException("HardStop должен выполняться в том же потоке, который останавливает");

        _targetThread.HardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _targetThread;

    public SoftStopCommand(ServerThread targetThread)
    {
        _targetThread = targetThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _targetThread.GetThread())
            throw new InvalidOperationException("SoftStop должен выполняться в том же потоке, который останавливает");

        _targetThread.SoftStop();
    }
}

public class TestCommand : ICommand
{
    private readonly Action _action;

    public TestCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}