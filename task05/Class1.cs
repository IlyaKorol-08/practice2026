using System.Reflection;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Select(m => m.Name)
                    .Distinct();
    }

    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var method = _type.GetMethod(methodName, 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        
        if (method == null)
        {
            return Enumerable.Empty<string>();
        }
        var parameters = method.GetParameters()
                               .Select(p => $"{p.ParameterType.Name} {p.Name}");
        
        var returnType = $"Return: {method.ReturnType.Name}";

        return parameters.Append(returnType);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | 
                               BindingFlags.Instance | BindingFlags.Static)
                    .Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | 
                                   BindingFlags.Instance | BindingFlags.Static)
                    .Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttributes(typeof(T), true).Any();
    }
}