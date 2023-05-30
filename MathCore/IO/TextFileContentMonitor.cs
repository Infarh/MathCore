using System.Text;

namespace MathCore.IO;

public class TextFileContentMonitor
{
    public event EventHandler<EventArgs<StringBuilder>> NewContent;

    protected virtual void OnNewContent(StringBuilder content) => NewContent?.Invoke(this, content);

    private readonly FileInfo _File;
    private readonly FileSystemWatcher _Watcher;
    private long _LastLength;

    public TextFileContentMonitor(string FileName) : this(new FileInfo(FileName)) { }

    public TextFileContentMonitor(FileInfo File)
    {
        _File = File;

        var length = File.Length;
        _LastLength = length;

        var directory = File.Directory.FullName;
        var file_name = File.Name;
        _Watcher = new FileSystemWatcher(directory, file_name)
        {
            NotifyFilter = NotifyFilters.LastWrite,
        };

        _Watcher.Changed += OnFileChanged;
    }

    public void Start()
    {
        if (_Watcher.EnableRaisingEvents)
            return;

        _ = ReadFileContentAsync();
        _Watcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var file = _File;
        file.Refresh();

        var current_length = file.Length;

        var last_length = _LastLength;
        var delta = current_length - last_length;

        if (delta > 0)
            _ = ReadFileContentAsync(last_length);

        _LastLength = current_length;
    }

    private async Task ReadFileContentAsync(long Offset = 0)
    {
        for (var retry_count = -1; retry_count < 5; retry_count++)
            try
            {
                var result = await ReadContentAsync(_File, Offset).ConfigureAwait(false);

                OnNewContent(result);
                break;
            }
            catch (IOException error) when (error.HResult == -2147024864)
            {
                if (_File.GetLockingProcesses() is { Count: > 0 } processes)
                    foreach (var process in processes.Where(p => !p.HasExited))
                        process.WaitForExit();
                else if (retry_count >= 0)
                    await Task.Delay(50 * (1 << retry_count));
            }
    }

    private async Task<StringBuilder> ReadContentAsync(FileInfo file, long Offset)
    {
        using var stream = _File.OpenRead();
        stream.Seek(Offset, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = new StringBuilder((int)(stream.Length - Offset));
        var reading_task = reader.ReadLineAsync();
        while (true)
        {
            if (await reading_task.ConfigureAwait(false) is not { } line)
                break;
            reading_task = reader.ReadLineAsync();
            result.AppendLine(line);
        }

        return result;
    }

    public IEnumerable<StringBuilder> EnumerateChanges()
    {
        var file = _File;
        file.Refresh();
        var last_offset = file.Length;

        if (last_offset > 0)
            yield return ReadContent(file, 0);

        while (true)
        {
            file.Refresh();
            var new_length = file.Length;
            var delta = new_length - last_offset;
            if (delta > 0)
                yield return ReadContent(file, last_offset);

            last_offset = new_length;
        }
    }

    private StringBuilder ReadContent(FileInfo file, long Offset)
    {
        IOException last_error = null!;
        for (var retry_count = -1; retry_count < 5; retry_count++)
            try
            {
                using var stream = _File.OpenRead();
                stream.Seek(Offset, SeekOrigin.Begin);

                using var reader = new StreamReader(stream);

                var result = new StringBuilder((int)(stream.Length - Offset));
                while (!reader.EndOfStream)
                    result.Append(reader.ReadLine());

                return result;
            }
            catch (IOException error) when (error.HResult == -2147024864)
            {
                last_error = error;

                if (_File.GetLockingProcesses() is { Count: > 0 } processes)
                    foreach (var process in processes.Where(p => !p.HasExited))
                        process.WaitForExit();
                else if (retry_count >= 0)
                    Thread.Sleep(50 * (1 << retry_count));
            }

        throw new InvalidOperationException("Не удалось получить доступ к файлу", last_error);
    }
}
