namespace VR.Interfaces
{
    public interface IFileMonitorService
    {
        void StartWatching();

        Task ProcessFileAsync(string filePath);
    }
}
