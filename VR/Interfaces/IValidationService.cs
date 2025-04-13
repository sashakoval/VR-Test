namespace VR.Interfaces
{
    public interface IValidationService
    {
        void ValidateHeader(string[] parts);
        void ValidateLine(string[] parts);
    }
}