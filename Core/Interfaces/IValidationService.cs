namespace Core.Interfaces
{
    /// <summary>
    /// Interface for validation services.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates the header line of the file.
        /// </summary>
        /// <param name="parts">The parts of the header line.</param>
        /// <exception cref="FormatException">Thrown when the header line is invalid.</exception>
        void ValidateHeader(string[] parts);

        /// <summary>
        /// Validates a line of the file.
        /// </summary>
        /// <param name="parts">The parts of the line.</param>
        /// <exception cref="FormatException">Thrown when the line is invalid.</exception>
        void ValidateLine(string[] parts);
    }
}
