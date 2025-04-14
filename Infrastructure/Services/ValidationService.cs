using Core.Interfaces;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service to validate the contents of the file being parsed.
    /// </summary>
    public class ValidationService : IValidationService
    {
        /// <summary>
        /// Validates the header line.
        /// </summary>
        /// <param name="parts">The parts of the header line.</param>
        public void ValidateHeader(string[] parts)
        {
            if (parts.Length != 3)
            {
                throw new FormatException($"Unexpected header format: {string.Join(' ', parts)}. Expected format: HDR <SupplierIdentifier> <BoxIdentifier>");
            }

            if (string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2]))
            {
                throw new FormatException($"Invalid header data: {string.Join(' ', parts)}. SupplierIdentifier and BoxIdentifier cannot be empty.");
            }
        }

        /// <summary>
        /// Validates the line.
        /// </summary>
        /// <param name="parts">The parts of the line.</param>
        public void ValidateLine(string[] parts)
        {
            if (parts.Length != 4)
            {
                throw new FormatException($"Unexpected line format: {string.Join(' ', parts)}. Expected format: LINE <PoNumber> <Isbn> <Quantity>");
            }

            if (!int.TryParse(parts[3], out int quantity) || quantity <= 0)
            {
                throw new FormatException($"Quantity must be valid and Quantity must be greater than 0.");
            }

            if (!IsValidPoNumber(parts[1]))
            {
                throw new FormatException($"Invalid PO number: {parts[1]}. PO number must start with a letter.");
            }

            if (!IsValidIsbn13(parts[2]))
            {
                throw new FormatException($"Invalid ISBN-13: {parts[2]}. ISBN-13 must be a 13-digit number.");
            }
        }

        private bool IsValidIsbn13(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                throw new FormatException($"PO number cannot be empty.");
            }

            isbn = isbn.Replace("-", "").Replace(" ", "");
            if (isbn.Length != 13) return false;
            int sum = 0;
            foreach (var (index, digit) in isbn.Select((digit, index) => (index, digit)))
            {
                if (char.IsDigit(digit)) sum += (digit - '0') * (index % 2 == 0 ? 1 : 3);
                else return false;
            }
            return sum % 10 == 0;
        }

        private bool IsValidPoNumber(string poNumber)
        {
            return !string.IsNullOrWhiteSpace(poNumber) && char.IsLetter(poNumber[0]);
        }
    }
}

