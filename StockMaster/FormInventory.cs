using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StockMaster
{
    public partial class FormInventory : Form
    {
        private DataGridView? dgv;

        
        public FormInventory()
        {
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Inventory Management";
            this.Size = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized; 


            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(layout);

            // Panel tombol
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };
            layout.Controls.Add(panelTop, 0, 0);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true
            };
            panelTop.Controls.Add(buttonPanel);

            // Tombol
            Button btnAdd = CreateButton("Tambah", BtnAdd_Click);
            Button btnEdit = CreateButton("Edit", BtnEdit_Click);
            Button btnDelete = CreateButton("Hapus", BtnDelete_Click);

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnEdit);
            buttonPanel.Controls.Add(btnDelete);

            // DataGridView
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 35 },
                Font = new Font("Segoe UI", 10),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(200, 200, 200),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                EnableHeadersVisualStyles = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 245)
                }
            };
            layout.Controls.Add(dgv, 0, 1);

            // Kolom tabel
            dgv.Columns.Add("inventoryId", "Inventory ID");
            dgv.Columns.Add("productId", "Product ID");
            dgv.Columns.Add("quantity", "Quantity");
            dgv.Columns.Add("date", "Date");
            dgv.Columns.Add("type", "Type");
        }

        private Button CreateButton(string text, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(80, 30),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Margin = new Padding(5)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.DodgerBlue;
            btn.MouseLeave += (s, e) => btn.BackColor = Color.LightBlue;

            return btn;
        }

        private void LoadData()
        {
            if (dgv == null) return;
            dgv.Rows.Clear();

            List<DatabaseHelper.Inventory> list = DatabaseHelper.GetInventory();
            foreach (var inv in list)
            {
                dgv.Rows.Add(inv.InventoryId, inv.ProductId, inv.Quantity, inv.Date, inv.Type);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using InventoryFormDialog dialog = new InventoryFormDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DatabaseHelper.InsertInventory(new DatabaseHelper.Inventory
                {
                    // ProductId = dialog.ProductId,
                    ProductId = int.TryParse(dialog.ProductId, out int p) ? p : 0,
                    Quantity = int.TryParse(dialog.Quantity, out int q) ? q : 0,
                    Date = dialog.Date,
                    Type = dialog.Type
                });
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];

                InventoryFormDialog dialog = new InventoryFormDialog
                {
                    InventoryId = row.Cells["inventoryId"]?.Value?.ToString(),
                    ProductId = row.Cells["productId"]?.Value?.ToString(),
                    Quantity = row.Cells["quantity"]?.Value?.ToString(),
                    Date = row.Cells["date"]?.Value?.ToString(),
                    Type = row.Cells["type"]?.Value?.ToString()
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DatabaseHelper.UpdateInventory(new DatabaseHelper.Inventory
                    {
                        InventoryId = int.TryParse(dialog.InventoryId, out int id) ? id : 0,
                        // ProductId = dialog.ProductId,
                        ProductId = int.TryParse(dialog.ProductId, out int p) ? p : 0,
                        Quantity = int.TryParse(dialog.Quantity, out int q) ? q : 0,
                        Date = dialog.Date,
                        Type = dialog.Type
                    });
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Pilih baris yang mau diedit.");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                var confirm = MessageBox.Show("Yakin mau hapus?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    string? idStr = row.Cells["inventoryId"]?.Value?.ToString();
                    if (int.TryParse(idStr, out int id))
                    {
                        DatabaseHelper.DeleteInventory(id);
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih baris yang mau dihapus.");
            }
        }
    }
}
