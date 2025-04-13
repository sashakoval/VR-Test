public static class IsbnGenerator
{
    /// <summary>
    /// Generates a valid ISBN-13 number.
    /// </summary>
    /// <returns>A valid ISBN-13 number.</returns>
    public static string GenerateIsbn13()
    {
        var random = new Random();
        var isbn = new int[12];
        for (int i = 0; i < 12; i++)
        {
            isbn[i] = random.Next(0, 10);
        }

        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            sum += isbn[i] * (i % 2 == 0 ? 1 : 3);
        }

        int checksum = (10 - (sum % 10)) % 10;
        return string.Concat(isbn.Select(d => d.ToString())) + checksum;
    }
}

