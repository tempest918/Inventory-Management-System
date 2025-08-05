namespace Inventory_Management_System
{
    using System.ComponentModel;

    namespace InventoryManagement
    {
        /// <summary>
        /// the class for a product in the inventory.
        /// </summary>
        public class Product
        {
            // we use a BindingList to allow for dynamic updates in the UI.
            public BindingList<Part> AssociatedParts { get; set; } = new BindingList<Part>();

            // list of the properties defined by the UML diagram.
            public int ProductID { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int InStock { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }

            /// <summary>
            /// add a part to the associated parts of the product.
            /// </summary>
            /// <param name="part"></param>
            public void addAssociatedPart(Part part)
            {
                AssociatedParts.Add(part);
            }

            /// <summary>
            /// remove a part from the associated parts of the product.
            /// </summary>
            /// <param name="partID">The ID of the part to remove.</param>
            /// <returns>True if the part was found and removed, otherwise returns false.</returns>
            public bool removeAssociatedPart(int partID)
            {
                Part partToRemove = lookupAssociatedPart(partID);
                if (partToRemove != null)
                {
                    AssociatedParts.Remove(partToRemove);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// search for a part in the associated parts of the product by its ID.
            /// </summary>
            /// <param name="partID">The ID of the part to find.</param>
            /// <returns>The Part object if found; otherwise, null.</returns>
            public Part lookupAssociatedPart(int partID)
            {
                foreach (Part part in AssociatedParts)
                {
                    if (part.PartID == partID)
                    {
                        return part;
                    }
                }
                return null;
            }
        }
    }

}
