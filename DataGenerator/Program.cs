using System.Text;

class Program
{
    /// <summary>
    /// Main entry point for the data generator program.
    /// </summary>
    static void Main()
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "generated_data");
        Directory.CreateDirectory(directoryPath);
        string filePath = Path.Combine(directoryPath, "datafile.txt");
        int numberOfBoxes = 1050;
        var random = new Random();

        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            for (int i = 1; i <= numberOfBoxes; i++)
            {
                string supplierIdentifier = $"Supplier{i}";
                string boxIdentifier = $"Box{i}";
                writer.WriteLine($"HDR {supplierIdentifier} {boxIdentifier}");

                int numberOfLines = random.Next(1, 5);
                for (int j = 1; j <= numberOfLines; j++)
                {
                    char firstLetter = (char)random.Next('A', 'Z' + 1);
                    string poNumber = $"{firstLetter}{random.Next(100000000, 999999999)}";
                    string isbn = IsbnGenerator.GenerateIsbn13();
                    int quantity = random.Next(1, 100);
                    writer.WriteLine($"LINE {poNumber} {isbn} {quantity}");
                }
            }
        }

        Console.WriteLine($"Data file '{filePath}' generated with {numberOfBoxes} boxes.");
    }
}

