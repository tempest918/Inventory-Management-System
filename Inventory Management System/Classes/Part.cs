namespace Inventory_Management_System
{
    /// <summary>
    /// this is the abstract base class for parts in the inventory.
    /// </summary>
    public abstract class Part
    {
        // These are the properties defined by the UML diagram.
        public int PartID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
