using System.Windows.Forms.DataVisualization.Charting;

namespace StockMaster;

public partial class FormAnalisis : Form
{
    private int totalProduk = 0;
    private int totalStok = 0;

    public FormAnalisis()
    {
        SetupUI();
        HitungData();
        TampilkanChart();
    }

    private void SetupUI()
    {
        this.Text = "Analisis Stock";
        this.Size = new Size(600, 400);

        // Label Total Produk
        Label lblTotalProduk = new Label
        {
            Name = "lblTotalProduk",
            Text = "Total Produk: 0",
            Font = new Font("Arial", 14, FontStyle.Bold),
            Location = new Point(30, 20),
            AutoSize = true
        };
        this.Controls.Add(lblTotalProduk);

        // Label Total Stok
        Label lblTotalStok = new Label
        {
            Name = "lblTotalStok",
            Text = "Total Stok: 0",
            Font = new Font("Arial", 14, FontStyle.Bold),
            Location = new Point(30, 60),
            AutoSize = true
        };
        this.Controls.Add(lblTotalStok);
    }

    private void HitungData()
    {

        List<(string id, string nama, int stok)> dataInventory = new()
        {
            ("I001", "Barang A", 10),
            ("I002", "Barang B", 20),
            ("I003", "Barang C", 15)
        };

        totalProduk = dataInventory.Count;
        totalStok = dataInventory.Sum(item => item.stok);

        // Update label
        var lblProduk = this.Controls.Find("lblTotalProduk", true).FirstOrDefault() as Label;
        var lblStok = this.Controls.Find("lblTotalStok", true).FirstOrDefault() as Label;

        lblProduk!.Text = $"Total Produk: {totalProduk}";
        lblStok!.Text = $"Total Stok: {totalStok}";
    }

    private void TampilkanChart()
    {
        // Chart Pie 
        Chart chartPie = new Chart
        {
            Size = new Size(250, 200),
            Location = new Point(30, 120)
        };

        ChartArea areaPie = new ChartArea();
        chartPie.ChartAreas.Add(areaPie);

        Series seriesPie = new Series
        {
            ChartType = SeriesChartType.Pie,
            IsValueShownAsLabel = true
        };

        // Data simulasi
        seriesPie.Points.AddXY("Barang A", 10);
        seriesPie.Points.AddXY("Barang B", 20);
        seriesPie.Points.AddXY("Barang C", 15);

        chartPie.Series.Add(seriesPie);
        this.Controls.Add(chartPie);

        // Chart Line (Perbandingan stok)
        Chart chartLine = new Chart
        {
            Size = new Size(250, 200),
            Location = new Point(300, 120)
        };

        ChartArea areaLine = new ChartArea();
        chartLine.ChartAreas.Add(areaLine);

        Series seriesLine = new Series
        {
            ChartType = SeriesChartType.Line,
            IsValueShownAsLabel = true
        };

        // Data simulasi
        seriesLine.Points.AddXY("Barang A", 10);
        seriesLine.Points.AddXY("Barang B", 20);
        seriesLine.Points.AddXY("Barang C", 15);

        chartLine.Series.Add(seriesLine);
        this.Controls.Add(chartLine);
    }

}
