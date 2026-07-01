using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        long size = command.GetDirectorySize(testDir);
        Assert.True(size > 0);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        string[] files = command.GetFiles();
        Assert.Single(files);
        Assert.EndsWith(".txt", files[0]);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_EmptyDirectory_ShouldReturnZero()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "EmptyDir");
        Directory.CreateDirectory(testDir);

        var command = new DirectorySizeCommand(testDir);
        long size = command.GetDirectorySize(testDir);
        Assert.Equal(0, size);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_NonExistentDirectory_RunWithoutErrors()
    {
        var command = new DirectorySizeCommand("NonExistentPath");
        command.Execute();
    }

    [Fact]
    public void FindFilesCommand_NoMatches_ShouldReturnEmpty()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        string[] files = command.GetFiles();
        Assert.Empty(files);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_NonExistentDirectory_RunWithoutErrors()
    {
        var command = new FindFilesCommand("NonExistentPath", "*.txt");
        command.Execute();
    }
}