using System.Text.Json.Serialization;
using System.Text.Json;

namespace task13;

public class Subject
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime BirthDate { get; set; }
    
    public List<Subject> Grades { get; set; } = new();
}

public class DateOnlyConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd.MM.yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();
        if (DateTime.TryParseExact(dateString, DateFormat, null, 
            System.Globalization.DateTimeStyles.None, out DateTime date))
            return date;
        throw new JsonException($"Неверный формат даты. Ожидается: {DateFormat}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}

public class StudentSerializer
{
    private readonly JsonSerializerOptions _options;

    public StudentSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // Сериализация: объект → JSON строка
    public string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    // Десериализация: JSON строка → объект + валидация
    public Student? Deserialize(string json)
    {
        var student = JsonSerializer.Deserialize<Student>(json, _options);
        if (student != null)
            Validate(student);
        return student;
    }

    // Сохранить в файл
    public void SaveToFile(Student student, string filePath)
    {
        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    // Загрузить из файла
    public Student? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }

    // Валидация данных
    private void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
            throw new ArgumentException("Имя не может быть пустым");
        if (string.IsNullOrWhiteSpace(student.LastName))
            throw new ArgumentException("Фамилия не может быть пустой");
        if (student.BirthDate > DateTime.Now)
            throw new ArgumentException("Дата рождения не может быть в будущем");
        if (student.BirthDate < new DateTime(1900, 1, 1))
            throw new ArgumentException("Дата рождения слишком старая");
        foreach (var grade in student.Grades)
        {
            if (grade.Grade < 1 || grade.Grade > 5)
                throw new ArgumentException($"Оценка по предмету '{grade.Name}' должна быть от 1 до 5");
        }
    }
}
