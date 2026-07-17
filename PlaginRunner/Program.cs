using System.Reflection;
using PluginLib;

string pluginsFolder = args.Length > 0 ? args[0] : "plugins";

if (!Directory.Exists(pluginsFolder))
{
    Console.WriteLine($"Папка '{pluginsFolder}' не найдена.");
    return;
}

var dllFiles = Directory.GetFiles(pluginsFolder, "*.dll");
var pluginTypes = new Dictionary<string, Type>();
var pluginDependencies = new Dictionary<string, string[]>();

foreach (var dll in dllFiles)
{
    try
    {
        var assembly = Assembly.LoadFrom(dll);

        var types = assembly.GetTypes()
            .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
                        && t.GetCustomAttribute<PluginLoadAttribute>() != null);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<PluginLoadAttribute>()!;
            pluginTypes[type.Name] = type;
            pluginDependencies[type.Name] = attr.DependsOn != null
                ? new[] { attr.DependsOn }
                : Array.Empty<string>();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка загрузки {Path.GetFileName(dll)}: {ex.Message}");
    }
}

if (pluginTypes.Count == 0)
{
    Console.WriteLine("Плагины не найдены.");
    return;
}

Console.WriteLine($"Найдено плагинов: {pluginTypes.Count}");

var sortedPlugins = TopologicalSort(pluginDependencies);

Console.WriteLine("\nПорядок загрузки плагинов:");
foreach (var pluginName in sortedPlugins)
{
    var type = pluginTypes[pluginName];
    Console.WriteLine($"  - {pluginName}");

    try
    {
        var command = Activator.CreateInstance(type) as ICommand;
        command?.Execute();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"    Ошибка выполнения: {ex.Message}");
    }
}

static List<string> TopologicalSort(Dictionary<string, string[]> dependencies)
{
    var result = new List<string>();
    var inDegree = new Dictionary<string, int>();
    var graph = new Dictionary<string, List<string>>();

    foreach (var node in dependencies.Keys)
    {
        inDegree[node] = 0;
        graph[node] = new List<string>();
    }

    foreach (var node in dependencies)
    {
        foreach (var dep in node.Value)
        {
            if (!graph.ContainsKey(dep))
            {
                graph[dep] = new List<string>();
                inDegree[dep] = 0;
            }
            graph[dep].Add(node.Key);
            inDegree[node.Key]++;
        }
    }

    var queue = new Queue<string>();
    foreach (var node in inDegree)
    {
        if (node.Value == 0 && dependencies.ContainsKey(node.Key))
            queue.Enqueue(node.Key);
    }

    while (queue.Count > 0)
    {
        var node = queue.Dequeue();
        result.Add(node);

        if (graph.ContainsKey(node))
        {
            foreach (var neighbor in graph[node])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }
    }

    return result;
}