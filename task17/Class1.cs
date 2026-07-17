using System.Collections.Concurrent;

namespace task17;

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly LinkedList<ICommand> _commands = new();
    private readonly object _lock = new();

    public void Add(ICommand cmd)
    {
        lock (_lock)
        {
            _commands.AddLast(cmd);
        }
    }

    public ICommand Select()
    {
        lock (_lock)
        {
            if (_commands.Count > 0)
            {
                var cmd = _commands.First!.Value;
                _commands.RemoveFirst();
                return cmd;
            }
            throw new InvalidOperationException("Нет команд в планировщике");
        }
    }

    public bool HasCommand()
    {
        lock (_lock)
        {
            return _commands.Count > 0;
        }
    }
}

public class LongRunningCommand : ICommand
{
    private int _stepsRemaining;
    private readonly IScheduler _scheduler;
    private readonly Action<int>? _onStep;

    public LongRunningCommand(int steps, IScheduler scheduler, Action<int>? onStep = null)
    {
        _stepsRemaining = steps;
        _scheduler = scheduler;
        _onStep = onStep;
    }

    public void Execute()
    {
        if (_stepsRemaining > 0)
        {
            _onStep?.Invoke(_stepsRemaining);
            _stepsRemaining--;

            if (_stepsRemaining > 0)
            {
                _scheduler.Add(this);
            }
        }
    }

    public bool IsFinished => _stepsRemaining == 0;
}

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IScheduler _scheduler;
    private readonly Thread _thread;
    private volatile bool _hardStop;
    private volatile bool _softStop;

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(Run) { IsBackground = true };
    }

    public void Start() => _thread.Start();
    public Thread GetThread() => _thread;
    public void Enqueue(ICommand command) => _queue.Add(command);
    public void HardStop() => _hardStop = true;
    public void SoftStop() => _softStop = true;

    private void Run()
    {
        while (!_hardStop)
        {
            if (_scheduler.HasCommand())
            {
                ExecuteCommand(_scheduler.Select());
                continue;
            }

            if (_queue.TryTake(out var cmd, TimeSpan.FromMilliseconds(50)))
            {
                ExecuteCommand(cmd);
                continue;
            }

            if (_softStop && _queue.Count == 0 && !_scheduler.HasCommand())
                break;
        }
    }

    private void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _target;

    public HardStopCommand(ServerThread target) => _target = target;

    public void Execute()
    {
        if (Thread.CurrentThread != _target.GetThread())
            throw new InvalidOperationException("HardStop только в своём потоке");

        _target.HardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _target;

    public SoftStopCommand(ServerThread target) => _target = target;

    public void Execute()
    {
        if (Thread.CurrentThread != _target.GetThread())
            throw new InvalidOperationException("SoftStop только в своём потоке");

        _target.SoftStop();
    }
}

public class TestCommand : ICommand
{
    private readonly Action _action;

    public TestCommand(Action action) => _action = action;

    public void Execute() => _action();
}

public class RepeatedCommand : ICommand
{
    private readonly int _id;
    private readonly int _maxCalls;
    private readonly IScheduler _scheduler;
    private int _counter;

    public RepeatedCommand(int id, int maxCalls, IScheduler scheduler)
    {
        _id = id;
        _maxCalls = maxCalls;
        _scheduler = scheduler;
        _counter = 0;
    }

    public void Execute()
    {
        _counter++;
        Console.WriteLine($"Поток {_id} вызов {_counter}");

        if (_counter < _maxCalls)
        {
            _scheduler.Add(this);
        }
    }
}