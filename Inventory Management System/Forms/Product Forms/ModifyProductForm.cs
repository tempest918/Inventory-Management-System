using Inventory_Management_System.InventoryManagement;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inventory_Management_System
{
    public partial class ModifyProductForm : Form
    {
        // temporary list to hold parts associated with the product being modified
        private BindingList<Part> tempAssociatedParts = new BindingList<Part>();
        private Product productToModify;

        public ModifyProductForm(Product selectedProduct)
        {
            InitializeComponent();
            productToModify = selectedProduct;
            // copy associated parts from the selected product to the temporary list
            foreach (Part part in selectedProduct.AssociatedParts)
            {
                tempAssociatedParts.Add(part);
            }
            SetupDataGridViews();
            PopulateData();

            // perform initial validation to set the initial state of the save button
            ValidateFields(null, null);
        }

        /// <summary>
        /// populates the form controls with data from the product being modified.
        /// </summary>
        private void PopulateData()
        {
            idTextBox.Text = productToModify.ProductID.ToString();
            nameTextBox.Text = productToModify.Name;
            inventoryTextBox.Text = productToModify.InStock.ToString();
            priceTextBox.Text = productToModify.Price.ToString();
            minTextBox.Text = productToModify.Min.ToString();
            maxTextBox.Text = productToModify.Max.ToString();
        }

        /// <summary>
        /// configures the DataGridViews for displaying parts.
        /// </summary>
        private void SetupDataGridViews()
        {
            // Top grid: All available parts from inventory.
            allCandidatePartsDataGridView.DataSource = Inventory.AllParts;
            allCandidatePartsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allCandidatePartsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";

            // Bottom grid: Parts associated with the product being modified.
            associatedPartsDataGridView.DataSource = tempAssociatedParts;
            associatedPartsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            associatedPartsDataGridView.Columns["Price"].DefaultCellStyle.Format = "c";

        }

        #region Validation
        /// <summary>
        /// validates all input fields on the form and enables/disables the save button.
        /// </summary>
        private void ValidateFields(object sender, EventArgs e)
        {
            // J1 Validation: check fields for valid input
            bool isNameValid = ValidateField(nameTextBox, !string.IsNullOrWhiteSpace(nameTextBox.Text), "Name cannot be empty.");
            bool isInventoryValid = ValidateField(inventoryTextBox, int.TryParse(inventoryTextBox.Text, out _), "Inventory must be a whole number.");
            bool isPriceValid = ValidateField(priceTextBox, decimal.TryParse(priceTextBox.Text, out _), "Price must be a decimal (e.g., 12.99).");
            bool isMinValid = ValidateField(minTextBox, int.TryParse(minTextBox.Text, out _), "Min must be a whole number.");
            bool isMaxValid = ValidateField(maxTextBox, int.TryParse(maxTextBox.Text, out _), "Max must be a whole number.");

            bool isLogicValid = true;

            // J2 Validation: check that min is not greater than max, and inventory is within min and max
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
                tempAssociatedParts.Add(selectedPart);
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
                var confirmResult = MessageBox.Show("Are you sure you want to remove this part?", "Confirm Removal", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    tempAssociatedParts.Remove(selectedPart);
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
            var updatedProduct = new Product
            {
                ProductID = int.Parse(idTextBox.Text),
                Name = nameTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                InStock = int.Parse(inventoryTextBox.Text),
                Min = int.Parse(minTextBox.Text),
                Max = int.Parse(maxTextBox.Text),
                AssociatedParts = this.tempAssociatedParts
            };

            Inventory.updateProduct(updatedProduct.ProductID, updatedProduct);
            this.Close();
            #endregion
        }
        #endregion
    }
}
