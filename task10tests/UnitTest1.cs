using PluginLib;
using System.Reflection;

public class PluginTests
{
    [Fact]
    public void HelloCommand_HasPluginLoadAttribute()
    {
        var type = typeof(HelloCommand);
        var attr = type.GetCustomAttribute<PluginLoadAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void GoodbyeCommand_HasPluginLoadAttributeWithDependency()
    {
        var type = typeof(GoodbyeCommand);
        var attr = type.GetCustomAttribute<PluginLoadAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("HelloCommand", attr.DependsOn);
    }

    [Fact]
    public void HelloCommand_ImplementsICommand()
    {
        Assert.True(typeof(ICommand).IsAssignableFrom(typeof(HelloCommand)));
    }

    [Fact]
    public void GoodbyeCommand_ImplementsICommand()
    {
        Assert.True(typeof(ICommand).IsAssignableFrom(typeof(GoodbyeCommand)));
    }

    [Fact]
    public void HelloCommand_Execute_RunWithoutErrors()
    {
        var command = new HelloCommand();
        command.Execute();
        Assert.True(true);
    }

    [Fact]
    public void GoodbyeCommand_Execute_RunWithoutErrors()
    {
        var command = new GoodbyeCommand();
        command.Execute();
        Assert.True(true);
    }
}