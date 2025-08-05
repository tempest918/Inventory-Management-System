using Inventory_Management_System.InventoryManagement;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetupDataGridViews();
        }

        /// <summary>
        /// configures the DataGridViews for parts and products.
        /// </summary>
        private void SetupDataGridViews()
        {
            // bind the parts and products DataGridViews
            partsDataGridView.DataSource = Inventory.AllParts;
            partsDataGridView.AutoGenerateColumns = true;
            partsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";

            productsDataGridView.DataSource = Inventory.Products;
            productsDataGridView.AutoGenerateColumns = true;
            productsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";
        }

        #region general buttons

        // not sure why this is needed since the window has a close button, but it is in the GUI mock up so here it is
        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region parts

        private void partsAddButton_Click(object sender, EventArgs e)
        {
            AddPartForm addPartForm = new AddPartForm();
            addPartForm.ShowDialog();
        }

        private void partsModifyButton_Click(object sender, EventArgs e)
        {
            if (partsDataGridView.CurrentRow == null || partsDataGridView.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Please select a part to modify.", "No Part Selected");
                return;
            }

            Part selectedPart = (Part)partsDataGridView.CurrentRow.DataBoundItem;
            ModifyPartForm modifyPartForm = new ModifyPartForm(selectedPart);
            modifyPartForm.ShowDialog();
        }

        private void partsDeleteButton_Click(object sender, EventArgs e)
        {
            if (partsDataGridView.CurrentRow == null || partsDataGridView.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Please select a part to delete.", "No Part Selected");
                return;
            }

            Part partToDelete = (Part)partsDataGridView.CurrentRow.DataBoundItem;

            // J3 Validation: Check if the part is associated with any products
            if (Inventory.Products.Any(p => p.AssociatedParts.Contains(partToDelete)))
            {
                MessageBox.Show("Cannot delete a part that is associated with a product.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // J4 Validation: prompt the user to confirm deletion
            var confirmResult = MessageBox.Show("Are you sure you want to delete this part?",
                                                 "Confirm Deletion",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Inventory.deletePart(partToDelete);
            }
        }

        private void partsSearchButton_Click(object sender, EventArgs e)
        {
            string searchText = partsSearchTextBox.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                partsDataGridView.ClearSelection();
                return;
            }

            bool found = false;

            // try to search by PartID first
            if (int.TryParse(searchText, out int partID))
            {
                Part foundPart = Inventory.lookupPart(partID);
                if (foundPart != null)
                {
                    foreach (DataGridViewRow row in partsDataGridView.Rows)
                    {
                        if (row.DataBoundItem is Part partInGrid && partInGrid.PartID == foundPart.PartID)
                        {
                            row.Selected = true;
                            partsDataGridView.CurrentCell = row.Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
            }
            else // if not an int then search by Name
            {
                Part foundPart = Inventory.AllParts.FirstOrDefault(p => p.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
                if (foundPart != null)
                {
                    foreach (DataGridViewRow row in partsDataGridView.Rows)
                    {
                        if (row.DataBoundItem is Part partInGrid && partInGrid.PartID == foundPart.PartID)
                        {
                            row.Selected = true;
                            partsDataGridView.CurrentCell = row.Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found)
            {
                MessageBox.Show("Part not found.", "Search Result");
            }
        }
        #endregion

        #region products

        private void productsAddButton_Click(object sender, EventArgs e)
        {
            AddProductForm addProductForm = new AddProductForm();
            addProductForm.ShowDialog();
        }

        private void productsModifyButton_Click(object sender, EventArgs e)
        {
            if (productsDataGridView.CurrentRow == null || productsDataGridView.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Please select a product to modify.", "No Product Selected");
                return;
            }

            Product selectedProduct = (Product)productsDataGridView.CurrentRow.DataBoundItem;
            ModifyProductForm modifyProductForm = new ModifyProductForm(selectedProduct);
            modifyProductForm.ShowDialog();
        }


        private void productsDeleteButton_Click(object sender, EventArgs e)
        {
            if (productsDataGridView.CurrentRow == null || productsDataGridView.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Please select a product to delete.", "No Product Selected");
                return;
            }

            Product productToDelete = (Product)productsDataGridView.CurrentRow.DataBoundItem;

            // J4 Verification: prompt the user to confirm deletion
            var confirmResult = MessageBox.Show("Are you sure you want to delete this product?",
                                                 "Confirm Deletion",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Inventory.removeProduct(productToDelete.ProductID);
            }
        }

        private void productsSearchButton_Click(object sender, EventArgs e)
        {
            string searchText = productsSearchTextBox.Text;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                productsDataGridView.ClearSelection();
                return;
            }

            bool found = false;

            // try to search by ProductID first
            if (int.TryParse(searchText, out int productID))
            {
                Product foundProduct = Inventory.lookupProduct(productID);
                if (foundProduct != null)
                {
                    foreach (DataGridViewRow row in productsDataGridView.Rows)
                    {
                        if (row.DataBoundItem is Product productInGrid && productInGrid.ProductID == foundProduct.ProductID)
                        {
                            row.Selected = true;
                            productsDataGridView.CurrentCell = row.Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
            }
            else // if not an int then search by Name
            {
                Product foundProduct = Inventory.Products.FirstOrDefault(p => p.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
                if (foundProduct != null)
                {
                    foreach (DataGridViewRow row in productsDataGridView.Rows)
                    {
                        if (row.DataBoundItem is Product productInGrid && productInGrid.ProductID == foundProduct.ProductID)
                        {
                            row.Selected = true;
                            productsDataGridView.CurrentCell = row.Cells[0];
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found)
            {
                MessageBox.Show("Product not found.", "Search Result");
            }
        }
        #endregion
    }
}
