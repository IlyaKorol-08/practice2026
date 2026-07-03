namespace PluginLib;

public interface ICommand
{
    void Execute();
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string? DependsOn { get; set; }
}

[PluginLoad]
public class HelloCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Hello!");
    }
}

[PluginLoad(DependsOn = "HelloCommand")]
public class GoodbyeCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Goodbye!");
    }
}