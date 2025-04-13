namespace VR.Models
{
    public class Box
    {
        public required string SupplierIdentifier { get; set; }
        public required string Identifier { get; set; }

        public List<Content> Contents { get; set; } = new List<Content>();

        public class Content
        {
            public required string PoNumber { get; set; }
            public required string Isbn { get; set; }
            public required int Quantity { get; set; }
        }
    }
}
