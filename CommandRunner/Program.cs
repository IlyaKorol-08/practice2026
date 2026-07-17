using CommandLib;
using System.Reflection;

string commandsDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

if (!File.Exists(commandsDllPath))
{
    Console.WriteLine("Библиотека FileSystemCommands.dll не найдена");
    return;
}

Assembly assembly;
try
{
    assembly = Assembly.LoadFrom(commandsDllPath);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка загрузки сборки: {ex.Message}");
    return;
}

var commandTypes = assembly.GetTypes()
    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .ToList();

if (commandTypes.Count == 0)
{
    Console.WriteLine("Команды не найдены");
    return;
}

foreach (var type in commandTypes)
{
    Console.WriteLine($"\nНайден тип команды: {type.Name}");

    var constructors = type.GetConstructors();
    if (constructors.Length == 0)
    {
        try
        {
            var command = Activator.CreateInstance(type) as ICommand;
            command?.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения команды {type.Name}: {ex.Message}");
        }
    }
    else
    {
        var constructor = constructors[0];
        var parameters = constructor.GetParameters();
        object[] ctorArgs = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Console.Write($"Введите {parameters[i].Name} ({parameters[i].ParameterType.Name}): ");
            string? input = Console.ReadLine();

            if (parameters[i].ParameterType == typeof(int))
            {
                ctorArgs[i] = int.Parse(input ?? "0");
            }
            else
            {
                ctorArgs[i] = input ?? string.Empty;
            }
        }

        try
        {
            var command = Activator.CreateInstance(type, ctorArgs) as ICommand;
            command?.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения команды {type.Name}: {ex.Message}");
        }
    }
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();