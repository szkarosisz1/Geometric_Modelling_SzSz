
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BSplineWPF
{
    public partial class MainWindow : Window
    {
        private List<Point> controlPoints;
        private Polyline bSplineCurve;

        public MainWindow()
        {
            InitializeComponent();
            controlPoints = new List<Point>
            {
                new Point(50, 100),
                new Point(150, 50),
                new Point(250, 150),
                new Point(350, 100),
                new Point(450, 200),
                new Point(550, 100),
                new Point(650, 50),
                new Point(750, 150)
            };

            bSplineCurve = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            Canvas.Children.Add(bSplineCurve);
            DrawCurve();
        }

        private void UpdateCurve(object sender, RoutedEventArgs e)
        {
            // Frissítjük a görbe rendjét
            int degree = (int)DegreeSlider.Value;
            DegreeLabel.Text = "Rend: " + degree;

            DrawCurve();
        }

        private void DrawCurve()
        {
            bSplineCurve.Points.Clear();

            int degree = (int)DegreeSlider.Value;
            List<Point> curvePoints = GenerateBSpline(controlPoints, degree);

            foreach (var point in curvePoints)
            {
                bSplineCurve.Points.Add(point);
            }
        }

        private List<Point> GenerateBSpline(List<Point> controlPoints, int degree)
        {
            // B-spline görbe generálásának algoritmusának implementálása
            List<Point> curvePoints = new List<Point>();

            // Paraméterek számítása (például a knot vektor és az alapfüggvények)
            // Ez a rész a B-spline görbe generálásának algoritmusától függ

            // Egyszerűbb példa, ahol a pontokat interpoláljuk:
            for (int i = 0; i < controlPoints.Count - degree; i++)
            {
                double x = 0;
                double y = 0;

                for (int j = 0; j < degree; j++)
                {
                    x += controlPoints[i + j].X;
                    y += controlPoints[i + j].Y;
                }

                curvePoints.Add(new Point(x / degree, y / degree));
            }

            return curvePoints;
        }
    }
}
