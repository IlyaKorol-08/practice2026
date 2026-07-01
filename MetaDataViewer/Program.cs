using System.Reflection;

if (args.Length==0)
{
    Console.WriteLine("Укажите путь к DLL файлу в аргументах командной строки (Пример: dotnet run -- path/library.dll)");
    return;
}

string dllPath = args[0];
if(!File.Exists(dllPath))
{
    Console.WriteLine($"Файл не найден: {dllPath}");
    return;
}

try
{
    Assembly assembly = Assembly.LoadFrom(dllPath);
    PrintAssemblyMetaData(assembly);
}
catch
{
    Console.WriteLine($"Ошибка загрузки сборки");
}

static void PrintAssemblyMetaData(Assembly assembly)
{
    var types = assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract)
    .OrderBy(t => t.Name);

    foreach(var type in types)
    {
        PrintTypeInfo(type);
    }
}
static void PrintTypeInfo(Type type)
{
    Console.WriteLine($"\nКласс: {type.Name}");
    var attributes = type.GetCustomAttributes(false);
    if(attributes.Length>0)
    {
        Console.WriteLine("Атрибуты класса:");
        foreach(var attr in attributes)
        {
            PrintAttributeInfo((Attribute)attr);
        }
    }

    var constructors = type.GetConstructors(BindingFlags.Public|BindingFlags.Instance);
    if(constructors.Length>0)
    {
        Console.WriteLine("Конструкторы: ");
        foreach(var cons in constructors)
        {
            PrintMethodInfo("cons", cons.GetParameters());
        }
    }

    var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
    if(methods.Length>0)
    {
        Console.WriteLine("Методы:");
        foreach(var method in methods)
        {
            var methodAttributes = method.GetCustomAttributes(false);
            foreach (var attr in methodAttributes)
            {
                PrintAttributeInfo((Attribute)attr);
            }
            PrintMethodInfo(method.Name, method.GetParameters(), method.ReturnType);

        }
    }
}
static void PrintMethodInfo(string name, ParameterInfo[]parameters, Type? returnType=null)
{
    string returnInfo = returnType != null ? $"{returnType.Name} " : "";
    Console.Write($"{returnInfo}{name}(");
    for(int i =0;i<parameters.Length;i++)
    {
        Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
        if(i<parameters.Length - 1)
        {
            Console.Write(", ");
        }
    }
    Console.WriteLine(")");
}
static void PrintAttributeInfo(Attribute attr)
{
    Console.WriteLine($"[{attr.GetType().Name}]");
    var properties = attr.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);
    foreach(var prop in properties)
    {
        if(prop.DeclaringType == attr.GetType())
        {
            var value = prop.GetValue(attr);
            Console.WriteLine($"{prop.Name} = {value}");
        }
    }
}