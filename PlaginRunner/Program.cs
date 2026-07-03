using System.Reflection;
using PluginLib;

string dllPath = "/home/ilya/practice2026/PluginLib/bin/Debug/net10.0/PluginLib.dll";
if(!File.Exists(dllPath))
{
    Console.WriteLine($"Файл не найден");
    return;
}

var assembly = Assembly.LoadFrom(dllPath);
var commands = assembly.GetTypes()
    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface)
    .Select(t => new
    {
        Type = t,
        Attr = t.GetCustomAttribute<PluginLoadAttribute>()
    })
    .Where(x => x.Attr != null)
    .ToList();

Console.WriteLine("Команды без зависимостей:");
foreach (var cmd in commands.Where(c => c.Attr!.DependsOn == null))
{
    var instance = Activator.CreateInstance(cmd.Type) as ICommand;
    instance?.Execute();
}

Console.WriteLine("\nКоманды с зависимостями:");
foreach (var cmd in commands.Where(c => c.Attr!.DependsOn != null))
{
    Console.WriteLine($"(зависит от: {cmd.Attr!.DependsOn})");
    var instance = Activator.CreateInstance(cmd.Type) as ICommand;
    instance?.Execute();
}