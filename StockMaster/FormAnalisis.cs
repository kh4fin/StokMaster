using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using Timer = System.Windows.Forms.Timer;

namespace StockMaster
{
    public partial class FormAnalisis : Form
    {
        private int totalProduk = 0;
        private int totalStok = 0;

        private Label? lblHeader;
        private DataGridView? dgvLaporan;
        private TextBox? txtFilter;
        private Button? btnRefresh;
        private readonly Dictionary<Control, Point> targetPositions = new();
        private readonly Timer animTimer = new();

        private List<(int productId, string nama, int stokMasuk, int stokKeluar)> dataInventory = new();
        private List<(int productId, string nama, int stokMasuk, int stokKeluar)> filteredData = new();

        public FormAnalisis()
        {
            this.WindowState = FormWindowState.Maximized;
            SetupUI();
            LoadData();

            this.Load += (s, e) => StartAnimatedLayout();
            this.Resize += (s, e) => StartAnimatedLayout();
        }

        private void SetupUI()
        {
            this.Text = "Analisis Stok";
            this.BackColor = Color.WhiteSmoke;

            lblHeader = new Label
            {
                Text = "\uD83D\uDCCA ANALISIS STOK BARANG",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.MidnightBlue,
                AutoSize = true
            };
            this.Controls.Add(lblHeader);

            Label lblTotalProduk = new Label
            {
                Name = "lblTotalProduk",
                Text = "Total Produk: 0",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = true
            };
            this.Controls.Add(lblTotalProduk);

            Label lblTotalStok = new Label
            {
                Name = "lblTotalStok",
                Text = "Total Stok: 0",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkOrange,
                AutoSize = true
            };
            this.Controls.Add(lblTotalStok);

            // Filter TextBox
            txtFilter = new TextBox
            {
                PlaceholderText = "🔍 Cari Produk...",
                Font = new Font("Segoe UI", 12),
                Width = 250
            };
            txtFilter.TextChanged += (s, e) => ApplyFilter();
            this.Controls.Add(txtFilter);

            // Tombol Refresh
            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => LoadData();
            this.Controls.Add(btnRefresh);
        }

        private void LoadData()
        {
            AmbilDataDariDatabase();
            ApplyFilter(); // Otomatis refresh semua tampilan
        }

        private void AmbilDataDariDatabase()
        {
            var daftarProduk = DatabaseHelper.GetProducts();
            var daftarInventory = DatabaseHelper.GetInventory();

            dataInventory.Clear();

            foreach (var p in daftarProduk)
            {
                int stokMasuk = daftarInventory
                    .Where(i => i.ProductId == p.ProductId && i.Type == "masuk")
                    .Sum(i => i.Quantity);

                int stokKeluar = daftarInventory
                    .Where(i => i.ProductId == p.ProductId && i.Type == "keluar")
                    .Sum(i => i.Quantity);

                dataInventory.Add((p.ProductId, p.ProductName ?? "", stokMasuk, stokKeluar));
            }
        }

        private void ApplyFilter()
        {
            string keyword = txtFilter?.Text.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(keyword))
                filteredData = new List<(int, string, int, int)>(dataInventory);
            else
                filteredData = dataInventory.Where(item =>
                    item.nama.ToLower().Contains(keyword) || item.productId.ToString().Contains(keyword)
                ).ToList();

            HitungData();
            TampilkanChart();
            TampilkanLaporan();
            StartAnimatedLayout();
        }

        private void HitungData()
        {
            totalProduk = filteredData.Count;
            totalStok = filteredData.Sum(item => item.stokMasuk - item.stokKeluar);

            var lblProduk = this.Controls.Find("lblTotalProduk", true).FirstOrDefault() as Label;
            var lblStok = this.Controls.Find("lblTotalStok", true).FirstOrDefault() as Label;

            if (lblProduk != null) lblProduk.Text = $"\uD83D\uDCE6 Total Produk: {totalProduk}";
            if (lblStok != null) lblStok.Text = $"\uD83D\uDCC8 Total Stok: {totalStok}";
        }

        private void TampilkanChart()
        {
            // Hapus chart lama
            var oldCharts = this.Controls.OfType<Chart>().ToList();
            foreach (var ch in oldCharts)
                this.Controls.Remove(ch);

            Chart chartPie = new Chart
            {
                Size = new Size(400, 300),
                BackColor = Color.White
            };

            ChartArea areaPie = new ChartArea { BackColor = Color.WhiteSmoke };
            chartPie.ChartAreas.Add(areaPie);

            Series seriesPie = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            foreach (var item in filteredData)
            {
                int stokSaatIni = item.stokMasuk - item.stokKeluar;
                int pointIndex = seriesPie.Points.AddXY(item.nama, stokSaatIni);
                seriesPie.Points[pointIndex].Label = $"{item.nama}\nStok: {stokSaatIni}";
            }

            chartPie.Series.Add(seriesPie);
            this.Controls.Add(chartPie);

            Chart chartLine = new Chart
            {
                Size = new Size(400, 300),
                BackColor = Color.White
            };

            ChartArea areaLine = new ChartArea
            {
                BackColor = Color.WhiteSmoke,
                AxisX = { Title = "Produk" },
                AxisY = { Title = "Jumlah Stok" }
            };
            chartLine.ChartAreas.Add(areaLine);

            Series seriesLine = new Series
            {
                ChartType = SeriesChartType.Line,
                IsValueShownAsLabel = true,
                BorderWidth = 3,
                Color = Color.MediumPurple,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            foreach (var item in filteredData)
            {
                int stokSaatIni = item.stokMasuk - item.stokKeluar;
                seriesLine.Points.AddXY(item.nama, stokSaatIni);
            }

            chartLine.Series.Add(seriesLine);
            this.Controls.Add(chartLine);
        }

        private void TampilkanLaporan()
        {
            if (dgvLaporan != null)
                this.Controls.Remove(dgvLaporan);

            dgvLaporan = new DataGridView
            {
                Size = new Size(800, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                ColumnCount = 5,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvLaporan.Columns[0].Name = "ID Produk";
            dgvLaporan.Columns[1].Name = "Nama Produk";
            dgvLaporan.Columns[2].Name = "Stok Masuk";
            dgvLaporan.Columns[3].Name = "Stok Keluar";
            dgvLaporan.Columns[4].Name = "Stok Saat Ini";

            foreach (var item in filteredData)
            {
                int stokSaatIni = item.stokMasuk - item.stokKeluar;
                dgvLaporan.Rows.Add(item.productId, item.nama, item.stokMasuk, item.stokKeluar, stokSaatIni);
            }

            dgvLaporan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvLaporan.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
            dgvLaporan.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn col in dgvLaporan.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            this.Controls.Add(dgvLaporan);
        }

        private void StartAnimatedLayout()
        {
            CalculateTargetPositions();
            animTimer.Interval = 10;
            animTimer.Tick -= AnimateLayout;
            animTimer.Tick += AnimateLayout;
            animTimer.Start();
        }

        private void CalculateTargetPositions()
        {
            targetPositions.Clear();
            int spacingY = 20;
            int currentY = 20;

            if (lblHeader != null)
            {
                targetPositions[lblHeader] = new Point((this.ClientSize.Width - lblHeader.Width) / 2, currentY);
                currentY += lblHeader.Height + spacingY;
            }

            var lblProduk = this.Controls.Find("lblTotalProduk", true).FirstOrDefault();
            if (lblProduk != null)
            {
                targetPositions[lblProduk] = new Point((this.ClientSize.Width - lblProduk.Width) / 2, currentY);
                currentY += lblProduk.Height + spacingY;
            }

            var lblStok = this.Controls.Find("lblTotalStok", true).FirstOrDefault();
            if (lblStok != null)
            {
                targetPositions[lblStok] = new Point((this.ClientSize.Width - lblStok.Width) / 2, currentY);
                currentY += lblStok.Height + spacingY;
            }

            if (txtFilter != null)
            {
                targetPositions[txtFilter] = new Point((this.ClientSize.Width - txtFilter.Width - 120) / 2, currentY);
            }

            if (btnRefresh != null)
            {
                targetPositions[btnRefresh] = new Point((this.ClientSize.Width + 120) / 2, currentY);
                currentY += btnRefresh.Height + spacingY;
            }

            var charts = this.Controls.OfType<Chart>().ToList();
            if (charts.Count == 2)
            {
                int totalWidth = charts[0].Width + charts[1].Width + 40;
                int startX = (this.ClientSize.Width - totalWidth) / 2;

                targetPositions[charts[0]] = new Point(startX, currentY);
                targetPositions[charts[1]] = new Point(startX + charts[0].Width + 40, currentY);
                currentY += charts[0].Height + spacingY;
            }

            if (dgvLaporan != null)
            {
                targetPositions[dgvLaporan] = new Point((this.ClientSize.Width - dgvLaporan.Width) / 2, currentY);
            }
        }

        private void AnimateLayout(object? sender, EventArgs e)
        {
            bool done = true;

            foreach (var pair in targetPositions)
            {
                var ctrl = pair.Key;
                var target = pair.Value;
                var current = ctrl.Location;

                int dx = (target.X - current.X) / 5;
                int dy = (target.Y - current.Y) / 5;

                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                {
                    ctrl.Location = new Point(current.X + dx, current.Y + dy);
                    done = false;
                }
                else
                {
                    ctrl.Location = target;
                }
            }

            if (done)
            {
                animTimer.Stop();
            }
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Linq;
// using System.Windows.Forms;
// using System.Data;
// using System.Windows.Forms.DataVisualization.Charting;
// using Timer = System.Windows.Forms.Timer;

// namespace StockMaster
// {
//     public partial class FormAnalisis : Form
//     {
//         private int totalProduk = 0;
//         private int totalStok = 0;

//         private Label? lblHeader;
//         private DataGridView? dgvLaporan;
//         private readonly Dictionary<Control, Point> targetPositions = new();
//         private readonly Timer animTimer = new();

//         // Data dari database
//         private List<(int productId, string nama, int stokMasuk, int stokKeluar)> dataInventory = new();

//         public FormAnalisis()
//         {
//             this.WindowState = FormWindowState.Maximized;
//             SetupUI();
//             AmbilDataDariDatabase();
//             HitungData();
//             TampilkanChart();
//             TampilkanLaporan();

//             this.Load += (s, e) => StartAnimatedLayout();
//             this.Resize += (s, e) => StartAnimatedLayout();
//         }

//         private void SetupUI()
//         {
//             this.Text = "Analisis Stok";
//             this.BackColor = Color.WhiteSmoke;

//             lblHeader = new Label
//             {
//                 Text = "\uD83D\uDCCA ANALISIS STOK BARANG",
//                 Font = new Font("Segoe UI", 24, FontStyle.Bold),
//                 ForeColor = Color.MidnightBlue,
//                 AutoSize = true
//             };
//             this.Controls.Add(lblHeader);

//             Label lblTotalProduk = new Label
//             {
//                 Name = "lblTotalProduk",
//                 Text = "Total Produk: 0",
//                 Font = new Font("Segoe UI", 16, FontStyle.Bold),
//                 ForeColor = Color.DarkGreen,
//                 AutoSize = true
//             };
//             this.Controls.Add(lblTotalProduk);

//             Label lblTotalStok = new Label
//             {
//                 Name = "lblTotalStok",
//                 Text = "Total Stok: 0",
//                 Font = new Font("Segoe UI", 16, FontStyle.Bold),
//                 ForeColor = Color.DarkOrange,
//                 AutoSize = true
//             };
//             this.Controls.Add(lblTotalStok);
//         }

//         private void AmbilDataDariDatabase()
//         {
//             var daftarProduk = DatabaseHelper.GetProducts();
//             var daftarInventory = DatabaseHelper.GetInventory();

//             dataInventory.Clear();

//             foreach (var p in daftarProduk)
//             {
//                 int stokMasuk = daftarInventory
//                     .Where(i => i.ProductId == p.ProductId && i.Type == "masuk")
//                     .Sum(i => i.Quantity);

//                 int stokKeluar = daftarInventory
//                     .Where(i => i.ProductId == p.ProductId && i.Type == "keluar")
//                     .Sum(i => i.Quantity);

//                 dataInventory.Add((p.ProductId, p.ProductName ?? "", stokMasuk, stokKeluar));
//             }
//         }

//         private void HitungData()
//         {
//             totalProduk = dataInventory.Count;
//             totalStok = dataInventory.Sum(item => item.stokMasuk - item.stokKeluar);

//             var lblProduk = this.Controls.Find("lblTotalProduk", true).FirstOrDefault() as Label;
//             var lblStok = this.Controls.Find("lblTotalStok", true).FirstOrDefault() as Label;

//             if (lblProduk != null) lblProduk.Text = $"\uD83D\uDCE6 Total Produk: {totalProduk}";
//             if (lblStok != null) lblStok.Text = $"\uD83D\uDCC8 Total Stok: {totalStok}";
//         }

//         private void TampilkanChart()
//         {
//             Chart chartPie = new Chart
//             {
//                 Size = new Size(400, 300),
//                 BackColor = Color.White
//             };

//             ChartArea areaPie = new ChartArea
//             {
//                 BackColor = Color.WhiteSmoke
//             };
//             chartPie.ChartAreas.Add(areaPie);

//             Series seriesPie = new Series
//             {
//                 ChartType = SeriesChartType.Pie,
//                 IsValueShownAsLabel = true,
//                 Font = new Font("Segoe UI", 10, FontStyle.Bold)
//             };

//             foreach (var item in dataInventory)
//             {
//                 int stokSaatIni = item.stokMasuk - item.stokKeluar;
//                 int pointIndex = seriesPie.Points.AddXY(item.nama, stokSaatIni);
//                 seriesPie.Points[pointIndex].Label = $"{item.nama}\nStok: {stokSaatIni}";
//             }

//             chartPie.Series.Add(seriesPie);
//             this.Controls.Add(chartPie);

//             Chart chartLine = new Chart
//             {
//                 Size = new Size(400, 300),
//                 BackColor = Color.White
//             };

//             ChartArea areaLine = new ChartArea
//             {
//                 BackColor = Color.WhiteSmoke,
//                 AxisX = { Title = "Produk" },
//                 AxisY = { Title = "Jumlah Stok" }
//             };
//             chartLine.ChartAreas.Add(areaLine);

//             Series seriesLine = new Series
//             {
//                 ChartType = SeriesChartType.Line,
//                 IsValueShownAsLabel = true,
//                 BorderWidth = 3,
//                 Color = Color.MediumPurple,
//                 Font = new Font("Segoe UI", 10, FontStyle.Bold)
//             };

//             foreach (var item in dataInventory)
//             {
//                 int stokSaatIni = item.stokMasuk - item.stokKeluar;
//                 seriesLine.Points.AddXY(item.nama, stokSaatIni);
//             }

//             chartLine.Series.Add(seriesLine);
//             this.Controls.Add(chartLine);
//         }

//         private void TampilkanLaporan()
//         {
//             dgvLaporan = new DataGridView
//             {
//                 Size = new Size(800, 200),
//                 ReadOnly = true,
//                 AllowUserToAddRows = false,
//                 ColumnCount = 5,
//                 BackgroundColor = Color.White,
//                 BorderStyle = BorderStyle.Fixed3D,
//                 AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
//             };

//             dgvLaporan.Columns[0].Name = "ID Produk";
//             dgvLaporan.Columns[1].Name = "Nama Produk";
//             dgvLaporan.Columns[2].Name = "Stok Masuk";
//             dgvLaporan.Columns[3].Name = "Stok Keluar";
//             dgvLaporan.Columns[4].Name = "Stok Saat Ini";

//             foreach (var item in dataInventory)
//             {
//                 int stokSaatIni = item.stokMasuk - item.stokKeluar;
//                 dgvLaporan.Rows.Add(item.productId, item.nama, item.stokMasuk, item.stokKeluar, stokSaatIni);
//             }

//             dgvLaporan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
//             dgvLaporan.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
//             dgvLaporan.EnableHeadersVisualStyles = false;

//             foreach (DataGridViewColumn col in dgvLaporan.Columns)
//             {
//                 col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
//                 col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
//             }

//             this.Controls.Add(dgvLaporan);
//         }

//         private void StartAnimatedLayout()
//         {
//             CalculateTargetPositions();
//             animTimer.Interval = 10;
//             animTimer.Tick -= AnimateLayout;
//             animTimer.Tick += AnimateLayout;
//             animTimer.Start();
//         }

//         private void CalculateTargetPositions()
//         {
//             targetPositions.Clear();
//             int spacingY = 20;
//             int currentY = 20;

//             if (lblHeader != null)
//             {
//                 targetPositions[lblHeader] = new Point((this.ClientSize.Width - lblHeader.Width) / 2, currentY);
//                 currentY += lblHeader.Height + spacingY;
//             }

//             var lblProduk = this.Controls.Find("lblTotalProduk", true).FirstOrDefault();
//             if (lblProduk != null)
//             {
//                 targetPositions[lblProduk] = new Point((this.ClientSize.Width - lblProduk.Width) / 2, currentY);
//                 currentY += lblProduk.Height + spacingY;
//             }

//             var lblStok = this.Controls.Find("lblTotalStok", true).FirstOrDefault();
//             if (lblStok != null)
//             {
//                 targetPositions[lblStok] = new Point((this.ClientSize.Width - lblStok.Width) / 2, currentY);
//                 currentY += lblStok.Height + spacingY;
//             }

//             var charts = this.Controls.OfType<Chart>().ToList();
//             if (charts.Count == 2)
//             {
//                 int totalWidth = charts[0].Width + charts[1].Width + 40;
//                 int startX = (this.ClientSize.Width - totalWidth) / 2;

//                 targetPositions[charts[0]] = new Point(startX, currentY);
//                 targetPositions[charts[1]] = new Point(startX + charts[0].Width + 40, currentY);
//                 currentY += charts[0].Height + spacingY;
//             }

//             if (dgvLaporan != null)
//             {
//                 targetPositions[dgvLaporan] = new Point((this.ClientSize.Width - dgvLaporan.Width) / 2, currentY);
//             }
//         }

//         private void AnimateLayout(object? sender, EventArgs e)
//         {
//             bool done = true;

//             foreach (var pair in targetPositions)
//             {
//                 var ctrl = pair.Key;
//                 var target = pair.Value;
//                 var current = ctrl.Location;

//                 int dx = (target.X - current.X) / 5;
//                 int dy = (target.Y - current.Y) / 5;

//                 if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
//                 {
//                     ctrl.Location = new Point(current.X + dx, current.Y + dy);
//                     done = false;
//                 }
//                 else
//                 {
//                     ctrl.Location = target;
//                 }
//             }

//             if (done)
//             {
//                 animTimer.Stop();
//             }
//         }
//     }
// }
