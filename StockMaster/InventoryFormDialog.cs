using System;
using System.Drawing;
using System.Windows.Forms;

namespace StockMaster
{
    public partial class InventoryFormDialog : Form
    {
        private ComboBox comboBoxProduct = null!;
        private ComboBox comboBoxType = null!;
        private NumericUpDown numericQuantity = null!;
        private DateTimePicker dateTimePicker = null!;
        private Button buttonSave = null!;


        public DatabaseHelper.Inventory? inventoryData;

        public InventoryFormDialog()
        {
            // InitializeComponent();
            SetupUI();
        }
        public string? InventoryId { get; set; }

        public string? ProductId
        {
            get => comboBoxProduct?.SelectedValue?.ToString();
            set
            {
                if (comboBoxProduct != null)
                    comboBoxProduct.SelectedValue = value;
            }
        }

        public string? Quantity
        {
            get => numericQuantity?.Value.ToString();
            set
            {
                if (numericQuantity != null && int.TryParse(value, out int q))
                    numericQuantity.Value = q;
            }
        }

        public string? Date
        {
            get => dateTimePicker?.Value.ToString("yyyy-MM-dd");
            set
            {
                if (dateTimePicker != null && DateTime.TryParse(value, out DateTime d))
                    dateTimePicker.Value = d;
            }
        }

        public string? Type
        {
            get => comboBoxType?.SelectedItem?.ToString();
            set
            {
                if (comboBoxType != null)
                    comboBoxType.SelectedItem = value;
            }
        }

        private void SetupUI()
        {
            this.Text = "Tambah Inventory";
            this.Size = new Size(350, 300);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(20),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            this.Controls.Add(layout);

            // Product Dropdown
            layout.Controls.Add(new Label { Text = "Produk", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            comboBoxProduct = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            layout.Controls.Add(comboBoxProduct, 1, 0);

            // Quantity
            layout.Controls.Add(new Label { Text = "Jumlah", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            numericQuantity = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 10000, Value = 1 };
            layout.Controls.Add(numericQuantity, 1, 1);

            // Date
            layout.Controls.Add(new Label { Text = "Tanggal", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
            dateTimePicker = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            layout.Controls.Add(dateTimePicker, 1, 2);

            // Type (In/Out)
            layout.Controls.Add(new Label { Text = "Tipe", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 3);
            comboBoxType = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxType.Items.AddRange(new string[] { "In", "Out" });
            layout.Controls.Add(comboBoxType, 1, 3);

            // Save Button
            buttonSave = new Button
            {
                Text = "Simpan",
                Dock = DockStyle.None,
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.Click += buttonSave_Click;

            Panel panelButton = new Panel { Dock = DockStyle.Fill };
            panelButton.Controls.Add(buttonSave);
            buttonSave.Location = new Point((panelButton.Width - buttonSave.Width) / 2, (panelButton.Height - buttonSave.Height) / 2);
            buttonSave.Anchor = AnchorStyles.None;
            layout.Controls.Add(panelButton, 0, 4);
            layout.SetColumnSpan(panelButton, 2);

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            this.Load += InventoryFormDialog_Load;
        }
        private void InventoryFormDialog_Load(object? sender, EventArgs e)
        {
            LoadProducts();

            if (inventoryData != null)
            {
                if (comboBoxProduct != null)
                    comboBoxProduct.SelectedValue = inventoryData.ProductId;

                if (numericQuantity != null)
                    numericQuantity.Value = inventoryData.Quantity;

                if (dateTimePicker != null && DateTime.TryParse(inventoryData.Date, out DateTime parsedDate))
                    dateTimePicker.Value = parsedDate;

                if (comboBoxType != null)
                    comboBoxType.SelectedItem = inventoryData.Type;

                // Set InventoryId (agar bisa dipakai saat edit)
                InventoryId = inventoryData.InventoryId.ToString();
            }
        }


        private void LoadProducts()
        {
            var products = DatabaseHelper.GetProducts();
            comboBoxProduct.DataSource = products;
            comboBoxProduct.DisplayMember = "ProductName";
            comboBoxProduct.ValueMember = "ProductId";
        }

        private void buttonSave_Click(object? sender, EventArgs e)
        {
            if (comboBoxProduct.SelectedItem == null || comboBoxType.SelectedItem == null)
            {
                MessageBox.Show("Mohon isi semua field!");
                return;
            }

            inventoryData = new DatabaseHelper.Inventory
            {
                ProductId = Convert.ToInt32(comboBoxProduct.SelectedValue),
                Quantity = (int)numericQuantity.Value,
                Date = dateTimePicker.Value.ToString("yyyy-MM-dd"),
                Type = comboBoxType.SelectedItem.ToString()
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
    
}
