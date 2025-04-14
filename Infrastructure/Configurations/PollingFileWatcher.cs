public class PollingFileWatcher
{
    private readonly string _path;
    private readonly string _filter;
    private readonly TimeSpan _interval;
    private readonly Dictionary<string, DateTime> _fileTimestamps;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler<FileSystemEventArgs>? Created;
    public event EventHandler<FileSystemEventArgs>? Changed;

    public PollingFileWatcher(string path, string filter, TimeSpan interval)
    {
        _path = path;
        _filter = filter;
        _interval = interval;
        _fileTimestamps = new Dictionary<string, DateTime>();
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => PollDirectory(_cancellationTokenSource.Token));
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    private async Task PollDirectory(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var files = Directory.GetFiles(_path, _filter);
            foreach (var file in files)
            {
                var lastWriteTime = File.GetLastWriteTime(file);
                if (!_fileTimestamps.ContainsKey(file))
                {
                    _fileTimestamps[file] = lastWriteTime;
                    Created?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Created, _path, Path.GetFileName(file)));
                }
                else if (_fileTimestamps[file] != lastWriteTime)
                {
                    _fileTimestamps[file] = lastWriteTime;
                    Changed?.Invoke(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, _path, Path.GetFileName(file)));
                }
            }

            await Task.Delay(_interval, cancellationToken);
        }
    }
}
