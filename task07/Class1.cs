using System.Reflection;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod()
    {
    }
}

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayName != null)
        {
            Console.WriteLine($"Отображаемое имя класса: {displayName.DisplayName}");
        }

        var version = type.GetCustomAttribute<VersionAttribute>();
        if (version != null)
        {
            Console.WriteLine($"Версия класса: {version.Major}.{version.Minor}");
        }

        Console.WriteLine("\nМетоды:");
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var methodAttr = method.GetCustomAttribute<DisplayNameAttribute>();
            if (methodAttr != null)
            {
                Console.WriteLine($"  - {methodAttr.DisplayName}");
            }
        }

        Console.WriteLine("\nСвойства:");
        foreach (var prop in type.GetProperties())
        {
            var propAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (propAttr != null)
            {
                Console.WriteLine($"  - {propAttr.DisplayName}");
            }
        }
    }
}