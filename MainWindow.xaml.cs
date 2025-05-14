using System;
using System.IO;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace StackedBarPdfExport
{
    public partial class MainWindow : Window
    {
        public PlotModel ChartModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            ChartModel = CreateStackedBarModel();
            PlotView.Model = ChartModel;
        }

        private PlotModel CreateStackedBarModel()
        {
            var model = new PlotModel { Title = "Quarterly Sales - Stacked Bar" };

            // Reverse axes to get vertical stacked bars (by default BarSeries are horizontal)
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            categoryAxis.Labels.Add("Q1");
            categoryAxis.Labels.Add("Q2");
            categoryAxis.Labels.Add("Q3");
            categoryAxis.Labels.Add("Q4");
            model.Axes.Add(categoryAxis);

            var valueAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Sales (k)" };
            model.Axes.Add(valueAxis);

            var productA = new BarSeries
            {
                Title = "Product A",
                IsStacked = true,
                StackGroup = "1",
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1
            };

            var productB = new BarSeries
            {
                Title = "Product B",
                IsStacked = true,
                StackGroup = "1",
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1
            };

            // Values for Product A
            productA.Items.Add(new BarItem { Value = 20 });
            productA.Items.Add(new BarItem { Value = 30 });
            productA.Items.Add(new BarItem { Value = 25 });
            productA.Items.Add(new BarItem { Value = 35 });

            // Values for Product B
            productB.Items.Add(new BarItem { Value = 15 });
            productB.Items.Add(new BarItem { Value = 20 });
            productB.Items.Add(new BarItem { Value = 30 });
            productB.Items.Add(new BarItem { Value = 25 });

            model.Series.Add(productA);
            model.Series.Add(productB);

            return model;
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            var exporter = new OxyPlot.Wpf.PngExporter
            {
                Width = 600,
                Height = 400,
                Background = OxyColors.White
            };
            byte[] pngBytes = exporter.ExportToBytes(ChartModel);
            string tempImagePath = Path.Combine(Path.GetTempPath(), "chart.png");
            File.WriteAllBytes(tempImagePath, pngBytes);

            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();
            page.Width = 600;
            page.Height = 450;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage image = XImage.FromFile(tempImagePath);
            gfx.DrawImage(image, 0, 0, page.Width, page.Height);

            string outputFile = "StackedBarChart.pdf";
            pdf.Save(outputFile);
            MessageBox.Show($"PDF saved at {Path.GetFullPath(outputFile)}", "Export Success");
        }
    }
}
