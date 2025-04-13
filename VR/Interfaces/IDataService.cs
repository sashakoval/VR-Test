using System.Collections.Generic;
using System.Threading.Tasks;
using VR.Models;

namespace VR.Interfaces
{
    public interface IDataService
    {
        Task SaveBoxesAsync(List<Box> boxes, int batchSize);
        Task<bool> BoxExistsAsync(string identifier);
    }
}
