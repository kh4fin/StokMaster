using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace StockMaster
{
    public partial class ProductFormDialog : Form
    {
        private TextBox? txtName, txtQuantity, txtPrice;
        private Button? btnSave;

        // [Browsable(false)]
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // public string? ProductId { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? NameProduct { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Category { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Price { get; set; }

        public ProductFormDialog()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Item Detail";
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

            // layout.Controls.Add(new Label { Text = "ID", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            // txtId = new TextBox { Dock = DockStyle.Fill };
            // layout.Controls.Add(txtId, 1, 0);

            layout.Controls.Add(new Label { Text = "Nama", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            txtName = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtName, 1, 1);

            layout.Controls.Add(new Label { Text = "Kategori", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
            txtQuantity = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtQuantity, 1, 2);

            layout.Controls.Add(new Label { Text = "Harga", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 3);
            txtPrice = new TextBox { Dock = DockStyle.Fill };
            layout.Controls.Add(txtPrice, 1, 3);

            btnSave = new Button
            {
                Text = "Simpan",
                Dock = DockStyle.None,
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.MediumSeaGreen;
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.LightGreen;

            Panel panelButton = new Panel { Dock = DockStyle.Fill };
            panelButton.Controls.Add(btnSave);
            btnSave.Location = new Point((panelButton.Width - btnSave.Width) / 2, (panelButton.Height - btnSave.Height) / 2);
            btnSave.Anchor = AnchorStyles.None;
            layout.Controls.Add(panelButton, 0, 4);
            layout.SetColumnSpan(panelButton, 2);

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // ProductId = txtId?.Text;
            NameProduct = txtName?.Text;
            Category = txtQuantity?.Text;
            Price = txtPrice?.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // if (txtId != null) txtId.Text = ProductId ?? "";
            if (txtName != null) txtName.Text = NameProduct ?? "";
            if (txtQuantity != null) txtQuantity.Text = Category ?? "";
            if (txtPrice != null) txtPrice.Text = Price ?? "";
        }
    }
}
