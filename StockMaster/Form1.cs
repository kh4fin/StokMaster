using System.Drawing;
using System.Windows.Forms;

namespace StockMaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = false;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Size = new Size(900, 600);
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "StockMaster";

            Panel header = new Panel
            {
                Size = new Size(this.Width, 80),
                BackColor = Color.FromArgb(50, 50, 50),
                Dock = DockStyle.Top
            };
            this.Controls.Add(header);

            PictureBox shopIcon = new PictureBox
            {
                ImageLocation = "./assets/store.png",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(50, 50),
                Location = new Point(10, 15)
            };
            header.Controls.Add(shopIcon);

            PictureBox profileIcon = new PictureBox
            {
                ImageLocation = "./assets/user_1.png",
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(header.Width - 60, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(profileIcon);

            FlowLayoutPanel panelCards = new FlowLayoutPanel
            {
                Padding = new Padding(30),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Anchor = AnchorStyles.None,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            Panel containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            containerPanel.Controls.Add(panelCards);

            containerPanel.Resize += (s, e) =>
            {
                panelCards.Location = new Point(
                    (containerPanel.ClientSize.Width - panelCards.PreferredSize.Width) / 2,
                    (containerPanel.ClientSize.Height - panelCards.PreferredSize.Height) / 2
                );
            };

            this.Load += (s, e) =>
            {
                panelCards.Location = new Point(
                    (containerPanel.ClientSize.Width - panelCards.PreferredSize.Width) / 2,
                    (containerPanel.ClientSize.Height - panelCards.PreferredSize.Height) / 2
                );
            };


            this.Controls.Add(containerPanel);

            panelCards.Controls.Add(CreateCard("Inventory", Color.LightBlue, InventoryCard_Click));

            panelCards.Controls.Add(CreateCard("Product", Color.LightGreen, ProductCard_Click));

            panelCards.Controls.Add(CreateCard("Analysis", Color.LightCoral, AnalysisCard_Click));
        }

        private Panel CreateCard(string title, Color color, EventHandler onClick)
        {
            Panel card = new Panel
            {
                Size = new Size(200, 150),
                BackColor = color,
                Margin = new Padding(20),
                Cursor = Cursors.Hand
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            card.Controls.Add(lblTitle);

            // Event click
            card.Click += onClick;
            lblTitle.Click += onClick;

            return card;
        }

        private void InventoryCard_Click(object? sender, EventArgs e)
        {
            FormInventory inventoryForm = new FormInventory();
            inventoryForm.ShowDialog();
        }

        private void ProductCard_Click(object? sender, EventArgs e)
        {
            FormProduct productForm = new FormProduct();
            productForm.ShowDialog();
        }

        private void AnalysisCard_Click(object? sender, EventArgs e)
        {
            FormAnalisis analisisForm = new FormAnalisis();
            analisisForm.ShowDialog();
        }
    }
}

