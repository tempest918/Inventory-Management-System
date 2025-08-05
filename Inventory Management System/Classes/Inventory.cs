using Inventory_Management_System.InventoryManagement;
using System.ComponentModel;
using System.Linq;

namespace Inventory_Management_System
{
    /// <summary>
    /// This class represents the inventory management system
    /// </summary>
    public class Inventory
    {
        // use binding lists for automatic UI updates
        public static BindingList<Product> Products { get; set; } = new BindingList<Product>();
        public static BindingList<Part> AllParts { get; set; } = new BindingList<Part>();

        #region product

        /// <summary>
        /// add a new product to the inventory.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public static void addProduct(Product product)
        {
            Products.Add(product);
        }

        /// <summary>
        /// remove a product from the inventory by its ID.
        /// </summary>
        /// <param name="productID">The ID of the product to remove.</param>
        /// <returns>True if removal was successful, false otherwise.</returns>
        public static bool removeProduct(int productID)
        {
            Product productToRemove = lookupProduct(productID);
            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// search for a product by its ID.
        /// </summary>
        /// <param name="productID">The ID of the product to find.</param>
        /// <returns>The found Product object, or null if not found.</returns>
        public static Product lookupProduct(int productID)
        {
            return Products.FirstOrDefault(p => p.ProductID == productID);
        }

        /// <summary>
        /// update a product in the inventory.
        /// </summary>
        /// <param name="productID">The ID of the product to update.</param>
        /// <param name="updatedProduct">The product object with updated information.</param>
        public static void updateProduct(int productID, Product updatedProduct)
        {
            Product existingProduct = lookupProduct(productID);
            if (existingProduct != null)
            {
                // findthe index of the existing product and replace it.
                int index = Products.IndexOf(existingProduct);
                Products[index] = updatedProduct;
            }
        }
        #endregion

        #region part

        /// <summary>
        /// adds a new part to the inventory.
        /// </summary>
        /// <param name="part">The part to add.</param>
        public static void addPart(Part part)
        {
            AllParts.Add(part);
        }

        /// <summary>
        /// deletes a part from the inventory by its ID.
        /// </summary>
        /// <param name="part">The part to delete.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        public static bool deletePart(Part part)
        {
            if (part != null)
            {
                AllParts.Remove(part);
                return true;
            }
            return false;
        }

        /// <summary>
        /// searches for a part in the inventory by its ID.
        /// </summary>
        /// <param name="partID">The ID of the part to find.</param>
        /// <returns>The found Part object, or null if not found.</returns>
        public static Part lookupPart(int partID)
        {
            return AllParts.FirstOrDefault(p => p.PartID == partID);
        }

        /// <summary>
        /// updates a part in the inventory.
        /// </summary>
        /// <param name="partID">The ID of the part to update.</param>
        /// <param name="updatedPart">The part object with updated information.</param>
        public static void updatePart(int partID, Part updatedPart)
        {
            Part existingPart = lookupPart(partID);
            if (existingPart != null)
            {
                // finsd the index of the existing part and replace it.
                int index = AllParts.IndexOf(existingPart);
                AllParts[index] = updatedPart;
            }
        }
        #endregion
    }
}
