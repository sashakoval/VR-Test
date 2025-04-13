namespace VR.Models
{
    /// <summary>
    /// Represents a box containing contents.
    /// </summary>
    public class Box
    {
        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        public required string SupplierIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the box identifier.
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the list of contents in the box.
        /// </summary>
        public List<Content> Contents { get; set; } = new List<Content>();

        /// <summary>
        /// Represents the content of a box.
        /// </summary>
        public class Content
        {
            /// <summary>
            /// Gets or sets the purchase order number.
            /// </summary>
            public required string PoNumber { get; set; }

            /// <summary>
            /// Gets or sets the ISBN number.
            /// </summary>
            public required string Isbn { get; set; }

            /// <summary>
            /// Gets or sets the quantity of the content.
            /// </summary>
            public required int Quantity { get; set; }
        }
    }
}
