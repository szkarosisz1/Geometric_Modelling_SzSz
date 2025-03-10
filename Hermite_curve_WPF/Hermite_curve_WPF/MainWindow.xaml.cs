using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Hermite_curve_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            // A felhasználói bemenetek beolvasása
            double p0x = double.Parse(Point1X.Text);
            double p0y = double.Parse(Point1Y.Text);
            double t0x = double.Parse(Tangent1X.Text);
            double t0y = double.Parse(Tangent1Y.Text);
            double p1x = double.Parse(Point2X.Text);
            double p1y = double.Parse(Point2Y.Text);
            double t1x = double.Parse(Tangent2X.Text);
            double t1y = double.Parse(Tangent2Y.Text);

            // A Hermite polinomok kiszámítása és kirajzolása
            Polyline hermiteCurve = new Polyline();
            hermiteCurve.Stroke = Brushes.Blue;
            hermiteCurve.StrokeThickness = 2;

            for (double t = 0; t <= 1; t += 0.01)
            {
                double h0 = 2 * Math.Pow(t, 3) - 3 * Math.Pow(t, 2) + 1;
                double h1 = -2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2);
                double h2 = Math.Pow(t, 3) - 2 * Math.Pow(t, 2) + t;
                double h3 = Math.Pow(t, 3) - Math.Pow(t, 2);

                double x = h0 * p0x + h1 * p1x + h2 * t0x + h3 * t1x;
                double y = h0 * p0y + h1 * p1y + h2 * t0y + h3 * t1y;

                hermiteCurve.Points.Add(new System.Windows.Point(x, y));
            }

            // A vászonra rajzolás
            DrawingCanvas.Children.Clear();
            DrawingCanvas.Children.Add(hermiteCurve);
        }
    }
}
