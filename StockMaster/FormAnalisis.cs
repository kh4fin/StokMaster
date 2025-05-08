using System;
using System.Drawing;
using System.Windows.Forms;

namespace StockMaster
{
    public partial class FormAnalisis : Form
    {
        private DataGridView? dgv;

        public FormAnalisis()
        {
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Inventory Analysis";
            this.Size = new Size(800, 600);
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

            // Button btnRefresh = CreateButton("Refresh", BtnRefresh_Click);
            // buttonPanel.Controls.Add(btnRefresh);

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

            dgv?.Columns.Add("indexId", "No"); 
            // dgv?.Columns.Add("inventoryId", "Inventory ID");
            dgv?.Columns.Add("productName", "Product Name");
            dgv?.Columns.Add("category", "Category");
            dgv?.Columns.Add("price", "Price");
            dgv?.Columns.Add("quantity", "Quantity");
            dgv?.Columns.Add("date", "Date");
            dgv?.Columns.Add("type", "Type");
        }

        // private Button CreateButton(string text, EventHandler onClick)
        // {
        //     Button btn = new Button
        //     {
        //         Text = text,
        //         Size = new Size(100, 30),
        //         BackColor = Color.LightBlue,
        //         FlatStyle = FlatStyle.Flat,
        //         Font = new Font("Segoe UI", 10, FontStyle.Regular),
        //         Margin = new Padding(5)
        //     };
        //     btn.FlatAppearance.BorderSize = 0;
        //     btn.Click += onClick;

        //     btn.MouseEnter += (s, e) => btn.BackColor = Color.DodgerBlue;
        //     btn.MouseLeave += (s, e) => btn.BackColor = Color.LightBlue;

        //     return btn;
        // }

        private void LoadData()
        {
            dgv?.Rows.Clear();
            var list = DatabaseHelper.GetInventoryWithProduct();

            int index = 1; 
            foreach (var item in list)
            {
                dgv?.Rows.Add(
                    index++, 
                    // item.InventoryId,
                    item.ProductName,
                    item.Category,
                    item.Price.ToString("N0"),
                    item.Quantity,
                    item.Date,
                    item.Type
                );
            }
        }

        // private void BtnRefresh_Click(object? sender, EventArgs e)
        // {
        //     LoadData();
        // }
    }
}
