using Inventory_Management_System.InventoryManagement;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Inventory_Management_System
{
    public partial class AddProductForm : Form
    {
        // temp list to hold parts associated with the new product
        private BindingList<Part> associatedPartsForNewProduct = new BindingList<Part>();

        public AddProductForm()
        {
            InitializeComponent();
            SetupDataGridViews();
            // autogen new product ID
            idTextBox.Text = GenerateNewProductID().ToString();

            // perform initial validation to set the initial state of the save button
            ValidateFields(null, null);
        }

        /// <summary>
        /// generates a new Product ID by finding the maximum existing Product ID and adding 1.
        /// </summary>
        private int GenerateNewProductID()
        {
            if (!Inventory.Products.Any())
            {
                return 0;
            }
            return Inventory.Products.Max(p => p.ProductID) + 1;
        }

        /// <summary>
        /// configures the DataGridViews for displaying parts.
        /// </summary>
        private void SetupDataGridViews()
        {
            // top grid: All candidate parts available for association with the new product
            allCandidatePartsDataGridView.DataSource = Inventory.AllParts;
            allCandidatePartsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allCandidatePartsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";

            // bottom grid: Associated parts for the new product
            associatedPartsDataGridView.DataSource = associatedPartsForNewProduct;
            associatedPartsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            associatedPartsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";
        }

        #region Validation
        /// <summary>
        /// validates all input fields on the form and enables/disables the save button.
        /// </summary>
        private void ValidateFields(object sender, EventArgs e)
        {
            // J1 Validation: ensure numeric values are in numeric fileds
            bool isNameValid = ValidateField(nameTextBox, !string.IsNullOrWhiteSpace(nameTextBox.Text), "Name cannot be empty.");
            bool isInventoryValid = ValidateField(inventoryTextBox, int.TryParse(inventoryTextBox.Text, out _), "Inventory must be a whole number.");
            bool isPriceValid = ValidateField(priceTextBox, decimal.TryParse(priceTextBox.Text, out _), "Price must be a decimal (e.g., 12.99).");
            bool isMinValid = ValidateField(minTextBox, int.TryParse(minTextBox.Text, out _), "Min must be a whole number.");
            bool isMaxValid = ValidateField(maxTextBox, int.TryParse(maxTextBox.Text, out _), "Max must be a whole number.");

            bool isLogicValid = true;

            // J2 Validation: min is less than max and inventory is between min and max
            if (isMinValid && isMaxValid && isInventoryValid)
            {
                int min = int.Parse(minTextBox.Text);
                int max = int.Parse(maxTextBox.Text);
                int inventory = int.Parse(inventoryTextBox.Text);

                if (min > max)
                {
                    isLogicValid = false;
                    ValidateField(minTextBox, false, "Min cannot be greater than Max.");
                    ValidateField(maxTextBox, false, "Max cannot be less than Min.");
                }
                else if (inventory < min || inventory > max)
                {
                    isLogicValid = false;
                    ValidateField(inventoryTextBox, false, "Inventory must be between Min and Max.");
                }
            }

            // enable the save button only if all validations pass
            saveButton.Enabled = isNameValid && isInventoryValid && isPriceValid && isMinValid && isMaxValid && isLogicValid;
        }

        /// <summary>
        /// helper method to set the visual state (color and tooltip) of a validated control.
        /// </summary>
        private bool ValidateField(Control control, bool isValid, string errorMessage)
        {
            if (isValid)
            {
                control.BackColor = Color.White;
                formToolTip.SetToolTip(control, "");
            }
            else
            {
                control.BackColor = Color.Salmon;
                formToolTip.SetToolTip(control, errorMessage);
            }
            return isValid;
        }
        #endregion

        #region events

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addPartButton_Click(object sender, EventArgs e)
        {
            if (allCandidatePartsDataGridView.CurrentRow?.DataBoundItem is Part selectedPart)
            {
                // add the selected part to the temporary list.
                associatedPartsForNewProduct.Add(selectedPart);
            }
            else
            {
                MessageBox.Show("Please select a part to add.", "No Part Selected");
            }
        }

        private void deletePartButton_Click(object sender, EventArgs e)
        {
            if (associatedPartsDataGridView.CurrentRow?.DataBoundItem is Part selectedPart)
            {
                // J4 Validation: Confirm removal of the selected part.
                var confirmResult = MessageBox.Show("Are you sure you want to remove this part?",
                                                     "Confirm Removal",
                                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // remove the selected part from the temporary list.
                    associatedPartsForNewProduct.Remove(selectedPart);
                }
            }
            else
            {
                MessageBox.Show("Please select a part to remove.", "No Part Selected");
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                allCandidatePartsDataGridView.ClearSelection();
                return;
            }

            if (int.TryParse(searchTextBox.Text, out int partID))
            {
                Part foundPart = Inventory.lookupPart(partID);
                if (foundPart != null)
                {
                    foreach (DataGridViewRow row in allCandidatePartsDataGridView.Rows)
                    {
                        if (row.DataBoundItem is Part partInGrid && partInGrid.PartID == foundPart.PartID)
                        {
                            row.Selected = true;
                            allCandidatePartsDataGridView.CurrentCell = row.Cells[0];
                            return;
                        }
                    }
                }
            }
            MessageBox.Show("Part not found.", "Search Result");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            #region save
            var newProduct = new Product
            {
                ProductID = int.Parse(idTextBox.Text),
                Name = nameTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                InStock = int.Parse(inventoryTextBox.Text),
                Min = int.Parse(minTextBox.Text),
                Max = int.Parse(maxTextBox.Text),
                // adding the associated parts from the temporary list
                AssociatedParts = this.associatedPartsForNewProduct
            };

            Inventory.addProduct(newProduct);
            this.Close();
            #endregion
        }
        #endregion
    }
}
