using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inventory_Management_System
{
    public partial class AddPartForm : Form
    {
        public AddPartForm()
        {
            InitializeComponent();
            // set initial state of the form
            inHouseRadioButton.Checked = true;
            // automatically generate a new Part ID
            idTextBox.Text = GenerateNewPartID().ToString();

            // perform initial validation to set the initial state of the save button
            ValidateFields(null, null);
        }

        /// <summary>
        /// generates a new Part ID by finding the maximum existing Part ID and adding 1.
        /// </summary>
        private int GenerateNewPartID()
        {
            // if there are no parsts in the inventory, return 0.
            if (!Inventory.AllParts.Any())
            {
                return 0;
            }
            // otherwise, find the maximum Part ID and add 1.
            return Inventory.AllParts.Max(p => p.PartID) + 1;
        }

        #region Validation
        /// <summary>
        /// validates all input fields on the form and enables/disables the save button.
        /// </summary>
        private void ValidateFields(object sender, EventArgs e)
        {
            // J1 Validation: make sure numeric fields are valid
            bool isNameValid = ValidateField(nameTextBox, !string.IsNullOrWhiteSpace(nameTextBox.Text), "Name cannot be empty.");
            bool isInventoryValid = ValidateField(inventoryTextBox, int.TryParse(inventoryTextBox.Text, out _), "Inventory must be a whole number.");
            bool isPriceValid = ValidateField(priceTextBox, decimal.TryParse(priceTextBox.Text, out _), "Price must be a decimal (e.g., 12.99).");
            bool isMinValid = ValidateField(minTextBox, int.TryParse(minTextBox.Text, out _), "Min must be a whole number.");
            bool isMaxValid = ValidateField(maxTextBox, int.TryParse(maxTextBox.Text, out _), "Max must be a whole number.");

            bool isMachineIdOrCompanyValid;
            if (inHouseRadioButton.Checked)
            {
                isMachineIdOrCompanyValid = ValidateField(machineIdOrCompanyTextBox, int.TryParse(machineIdOrCompanyTextBox.Text, out _), "Machine ID must be a number.");
            }
            else
            {
                isMachineIdOrCompanyValid = ValidateField(machineIdOrCompanyTextBox, !string.IsNullOrWhiteSpace(machineIdOrCompanyTextBox.Text), "Company Name cannot be empty.");
            }

            bool isLogicValid = true;
            // J2 Validation: check if min is less than max and if inventory is within min and max
            if (isMinValid && isMaxValid && isInventoryValid)
            {
                int min = int.Parse(minTextBox.Text);
                int max = int.Parse(maxTextBox.Text);
                int inventory = int.Parse(inventoryTextBox.Text);

                // J2 Validation: check if min is less than max
                if (min > max)
                {
                    isLogicValid = false;
                    ValidateField(minTextBox, false, "Min cannot be greater than Max.");
                    ValidateField(maxTextBox, false, "Max cannot be less than Min.");
                }
                // J2 Validation: check if inventory is within min and max
                else if (inventory < min || inventory > max)
                {
                    isLogicValid = false;
                    ValidateField(inventoryTextBox, false, "Inventory must be between Min and Max.");
                }
            }

            // enable the save button only if all validations pass
            saveButton.Enabled = isNameValid && isInventoryValid && isPriceValid && isMinValid && isMaxValid && isMachineIdOrCompanyValid && isLogicValid;
        }

        /// <summary>
        /// helper method to set the visual state (color and tooltip) of a validated control.
        /// </summary>
        /// <param name="control">The TextBox to validate.</param>
        /// <param name="isValid">The result of the validation check.</param>
        /// <param name="errorMessage">The tooltip message to show on error.</param>
        /// <returns>The value of isValid.</returns>
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            #region save
            // is in-house
            if (inHouseRadioButton.Checked)
            {
                Inventory.addPart(new Inhouse
                {
                    PartID = int.Parse(idTextBox.Text),
                    Name = nameTextBox.Text,
                    Price = decimal.Parse(priceTextBox.Text),
                    InStock = int.Parse(inventoryTextBox.Text),
                    Min = int.Parse(minTextBox.Text),
                    Max = int.Parse(maxTextBox.Text),
                    MachineID = int.Parse(machineIdOrCompanyTextBox.Text)
                });
            }
            else // is outsourced
            {
                Inventory.addPart(new Outsourced
                {
                    PartID = int.Parse(idTextBox.Text),
                    Name = nameTextBox.Text,
                    Price = decimal.Parse(priceTextBox.Text),
                    InStock = int.Parse(inventoryTextBox.Text),
                    Min = int.Parse(minTextBox.Text),
                    Max = int.Parse(maxTextBox.Text),
                    CompanyName = machineIdOrCompanyTextBox.Text
                });
            }

            this.Close();
            #endregion
        }

        // radio button checked changed event handler, changes the label text based on the selected radio button.
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (inHouseRadioButton.Checked)
            {
                machineIdOrCompanyLabel.Text = "Machine ID";
            }
            else
            {
                machineIdOrCompanyLabel.Text = "Company Name";
            }

            ValidateFields(sender, e);
        }
        #endregion
    }
}
