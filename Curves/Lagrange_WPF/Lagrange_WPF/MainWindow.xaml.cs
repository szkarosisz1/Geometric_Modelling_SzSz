using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LagrangeInterpolationWPF
{
    public partial class MainWindow : Window
    {
        private List<Ellipse> points = new List<Ellipse>();
        private List<Point> coordinates = new List<Point>();
        private Polyline interpolationCurve = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };
        private Ellipse selectedPoint = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            MyCanvas.Children.Add(interpolationCurve);
            for (int i = 0; i < 4; i++)
            {
                Point p = new Point(100 + i * 100, 200);
                coordinates.Add(p);
                Ellipse point = CreateDraggablePoint(p);
                points.Add(point);
                MyCanvas.Children.Add(point);
            }
            DrawLagrangeCurve();
        }

        private Ellipse CreateDraggablePoint(Point position)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Stroke = Brushes.Black
            };
            Canvas.SetLeft(ellipse, position.X - 5);
            Canvas.SetTop(ellipse, position.Y - 5);
            ellipse.MouseLeftButtonDown += Point_MouseDown;
            ellipse.MouseMove += Point_MouseMove;
            ellipse.MouseLeftButtonUp += Point_MouseUp;
            return ellipse;
        }

        private void Point_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedPoint = sender as Ellipse;
            selectedPoint.CaptureMouse();
        }

        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPoint != null)
            {
                Point newPosition = e.GetPosition(MyCanvas);
                Canvas.SetLeft(selectedPoint, newPosition.X - 5);
                Canvas.SetTop(selectedPoint, newPosition.Y - 5);
                int index = points.IndexOf(selectedPoint);
                coordinates[index] = newPosition;
                DrawLagrangeCurve();
            }
        }

        private void Point_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedPoint.ReleaseMouseCapture();
            selectedPoint = null;
        }

        private void DrawLagrangeCurve()
        {
            interpolationCurve.Points.Clear();
            for (double x = 0; x < MyCanvas.ActualWidth; x += 1)
            {
                double y = LagrangeInterpolation(x);
                interpolationCurve.Points.Add(new Point(x, y));
            }
        }

        private double LagrangeInterpolation(double x)
        {
            double result = 0;
            for (int i = 0; i < coordinates.Count; i++)
            {
                double term = coordinates[i].Y;
                for (int j = 0; j < coordinates.Count; j++)
                {
                    if (i != j)
                    {
                        term *= (x - coordinates[j].X) / (coordinates[i].X - coordinates[j].X);
                    }
                }
                result += term;
            }
            return result;
        }
    }
}
