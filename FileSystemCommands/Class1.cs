using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath)
    {
        _directoryPath = directoryPath; 
    }
    public void Execute()
    {
        if(!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Каталог не найден: {_directoryPath}");
            return;
        }

        long totalSize = GetDirectorySize(_directoryPath);
        Console.WriteLine($"Размер каталога: {_directoryPath}: {totalSize} байт");
    }
    public long GetDirectorySize(string path)
    {
        long size = 0;
        foreach (string file in Directory.GetFiles(path))
        {
            FileInfo fi = new FileInfo(file);
            size += fi.Length;
        }
        foreach(string dir in Directory.GetDirectories(path))
        {
            size += GetDirectorySize(dir);
        }

        return size;
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }
    public void Execute()
    {
        if(!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Каталог не найден{_directoryPath}");
            return;
        }
        string[]files = Directory.GetFiles(_directoryPath, _searchPattern);
        Console.WriteLine($"Найдено файлов по маске {_searchPattern}: {files.Length}");

        foreach (string file in files)
        {
            Console.WriteLine(Path.GetFileName(file));
        }
    }
    public string[]GetFiles()
    {
        if(!Directory.Exists(_directoryPath))
        {
            return Array.Empty<string>();
        }
        return Directory.GetFiles(_directoryPath, _searchPattern);
    }
}
