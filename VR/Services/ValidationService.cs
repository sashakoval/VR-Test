﻿using VR.Interfaces;

namespace VR.Services
{
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

        /// <summary>
        /// Checks if the given ISBN-13 is valid.
        /// </summary>
        /// <param name="isbn">The ISBN-13 to check.</param>
        /// <returns>True if the ISBN-13 is valid, otherwise false.</returns>
        public bool IsValidIsbn13(string isbn)
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

        /// <summary>
        /// Checks if the given PO number is valid.
        /// </summary>
        /// <param name="poNumber">The PO number to check.</param>
        /// <returns>True if the PO number is valid, otherwise false.</returns>
        public bool IsValidPoNumber(string poNumber)
        {
            return !string.IsNullOrWhiteSpace(poNumber) && char.IsLetter(poNumber[0]);
        }
    }
}

