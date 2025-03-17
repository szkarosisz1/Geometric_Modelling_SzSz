using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierCurveApp
{
    public partial class MainWindow : Window
    {
        private List<Ellipse> controlPoints = new List<Ellipse>();
        private List<Point> points = new List<Point>();
        private Dictionary<Point, double> weights = new Dictionary<Point, double>();
        private Polyline bezierCurve;

        public MainWindow()
        {
            InitializeComponent();
            bezierCurve = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };
            BezierCanvas.Children.Add(bezierCurve);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(BezierCanvas);
            AddControlPoint(clickPoint);
            DrawBezierCurve();
        }

        private void AddControlPoint(Point point)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
            };
            Canvas.SetLeft(ellipse, point.X - 5);
            Canvas.SetTop(ellipse, point.Y - 5);
            BezierCanvas.Children.Add(ellipse);
            controlPoints.Add(ellipse);
            points.Add(point);
            weights[point] = 1.0; // Default weight
        }

        private void DrawBezierCurve()
        {
            if (points.Count < 2) return;

            bezierCurve.Points.Clear();
            for (double t = 0; t <= 1; t += 0.01)
            {
                bezierCurve.Points.Add(ComputeRationalBezierPoint(t));
            }
        }

        private Point ComputeRationalBezierPoint(double t)
        {
            int n = points.Count - 1;
            double sumX = 0, sumY = 0, sumW = 0;

            for (int i = 0; i <= n; i++)
            {
                double b = Bernstein(n, i, t) * weights[points[i]];
                sumX += b * points[i].X;
                sumY += b * points[i].Y;
                sumW += b;
            }

            return new Point(sumX / sumW, sumY / sumW);
        }

        private double Bernstein(int n, int i, double t)
        {
            return BinomialCoeff(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
        }

        private int BinomialCoeff(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            int res = 1;
            for (int i = 1; i <= k; i++)
            {
                res *= (n - i + 1);
                res /= i;
            }
            return res;
        }
    }
}
