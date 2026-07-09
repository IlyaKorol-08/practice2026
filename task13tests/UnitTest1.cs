using task13;
using System.Text.Json;

namespace task13tests;

public class SerializationTests
{
    private readonly StudentSerializer _serializer;
    private readonly Student _testStudent;

    public SerializationTests()
    {
        _serializer = new StudentSerializer();
        _testStudent = new Student
        {
            FirstName = "Ilya",
            LastName = "Korol",
            BirthDate = new DateTime(2008, 1, 7),
            Grades = new List<Subject>
            {
                new Subject { Name = "Math", Grade = 5 },
                new Subject { Name = "Phy", Grade = 4 }
            }
        };
    }

    [Fact]
    public void Serialize_ReturnsValidJson()
    {
        string json = _serializer.Serialize(_testStudent);
        Assert.NotNull(json);
        Assert.Contains("Ilya", json);
        Assert.Contains("07.01.2008", json);
    }

    [Fact]
    public void Deserialize_ReturnsCorrectObject()
    {
        string json = _serializer.Serialize(_testStudent);
        var student = _serializer.Deserialize(json);
        Assert.NotNull(student);
        Assert.Equal("Ilya", student!.FirstName);
        Assert.Equal(new DateTime(2008, 1, 7), student.BirthDate);
        Assert.Equal(2, student.Grades.Count);
    }

    [Fact]
    public void SaveAndLoad_FileOperations_WorkCorrectly()
    {
        string filePath = Path.GetTempFileName();
        _serializer.SaveToFile(_testStudent, filePath);
        var loaded = _serializer.LoadFromFile(filePath);
        Assert.NotNull(loaded);
        Assert.Equal(_testStudent.FirstName, loaded!.FirstName);
        File.Delete(filePath);
    }

    [Fact]
    public void Deserialize_InvalidDate_ThrowsException()
    {
        string json = @"{""firstName"":""Ilya"",""lastName"":""Korol"",""birthDate"":""not-date"",""grades"":[]}";
        Assert.Throws<JsonException>(() => _serializer.Deserialize(json));
    }

    [Fact]
    public void Deserialize_EmptyName_ThrowsException()
    {
        string json = @"{""firstName"":"""",""lastName"":""Korol"",""birthDate"":""07.01.2008"",""grades"":[]}";
        Assert.Throws<ArgumentException>(() => _serializer.Deserialize(json));
    }
}