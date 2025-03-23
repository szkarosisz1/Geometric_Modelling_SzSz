using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_de_Casteljau_WPF
{
    public partial class MainWindow : Window
    {
        // Kezdeti kontrollpontok listája
        private List<Point> controlPoints = new List<Point>();
        private double t = 0.5;
        private Point bezierPoint;
        private int selectedPointIndex = -1;  // Kiválasztott pont indexe

        public MainWindow()
        {
            InitializeComponent();
            DrawBezier();
            AddMouseEvents();
        }

        // Egér események hozzáadása
        private void AddMouseEvents()
        {
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
        }

        // Az egér kattintása
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(canvas);
            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (IsNear(controlPoints[i], mousePosition))
                {
                    selectedPointIndex = i;
                    break;
                }
            }
        }

        // Az egér mozgatása
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPointIndex != -1)
            {
                Point newPosition = e.GetPosition(canvas);
                controlPoints[selectedPointIndex] = newPosition;  // Frissítjük a kiválasztott kontrollpontot
                DrawBezier();  // Újratervezzük a Bezier görbét
            }
        }

        // Az egér elengedése
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedPointIndex = -1;
        }

        // Képernyőn lévő kontrollpontok közeli állapotának ellenőrzése
        private bool IsNear(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) < 10 && Math.Abs(p1.Y - p2.Y) < 10;
        }

        // Rajzolás
        private void DrawBezier()
        {
            canvas.Children.Clear();
            DrawControlPolygon();
            DrawBezierCurve();
            bezierPoint = CalculateBezierPoint(t);
            DrawBezierPoint();
            DrawAuxiliaryLines();  // Segédvonalak
        }

        // Kontrollpontok szakaszainak rajzolása
        private void DrawControlPolygon()
        {
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                DrawLine(controlPoints[i], controlPoints[i + 1], Brushes.Gray);
            }

            foreach (var point in controlPoints)
            {
                DrawPoint(point, Brushes.Blue);
            }
        }

        // Bezier görbe rajzolása
        private void DrawBezierCurve()
        {
            Polyline bezierCurve = new Polyline { Stroke = Brushes.Black, StrokeThickness = 2 };
            for (double i = 0; i <= 1; i += 0.01)
            {
                bezierCurve.Points.Add(CalculateBezierPoint(i));
            }
            canvas.Children.Add(bezierCurve);
        }

        // de Casteljau algoritmus alkalmazása a t paraméterhez tartozó Bezier pont kiszámításához
        private Point CalculateBezierPoint(double t)
        {
            var points = new List<Point>(controlPoints);

            // Addig csökkentjük a pontok számát, amíg csak egy pont marad
            while (points.Count > 1)
            {
                List<Point> nextPoints = new List<Point>();

                // Segédvonalak rajzolása
                for (int i = 0; i < points.Count - 1; i++)
                {
                    // Kiszámítjuk a következő szint pontjait a de Casteljau algoritmus alapján
                    double x = (1 - t) * points[i].X + t * points[i + 1].X;
                    double y = (1 - t) * points[i].Y + t * points[i + 1].Y;
                    nextPoints.Add(new Point(x, y));

                    // Segédvonalak rajzolása, ha legalább két pont van
                    if (points.Count > 1)
                    {
                        DrawLine(points[i], points[i + 1], Brushes.LightGray); // Segédvonalak
                    }
                }

                // Az új pontok listája lesz a következő iteráció alapja
                points = nextPoints;
            }

            // Ha van legalább egy pont, akkor visszaadjuk
            if (points.Count > 0)
            {
                return points[0];
            }

            // Ha üres lenne a lista, visszaadunk egy alapértelmezett pontot
            return new Point(0, 0);
        }



        // Segédvonalak kirajzolása
        private void DrawAuxiliaryLines()
        {
            var points = new List<Point>(controlPoints);
            List<List<Point>> allIntermediatePoints = new List<List<Point>> { new List<Point>(points) };

            while (points.Count > 1)
            {
                List<Point> nextPoints = new List<Point>();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    double x = (1 - t) * points[i].X + t * points[i + 1].X;
                    double y = (1 - t) * points[i].Y + t * points[i + 1].Y;
                    nextPoints.Add(new Point(x, y));
                }
                points = nextPoints;
                allIntermediatePoints.Add(new List<Point>(points));
            }

            foreach (var intermediatePoints in allIntermediatePoints)
            {
                for (int i = 0; i < intermediatePoints.Count - 1; i++)
                {
                    DrawLine(intermediatePoints[i], intermediatePoints[i + 1], Brushes.Red);
                }
            }
        }

        // Bezier pont kirajzolása
        private void DrawBezierPoint()
        {
            DrawPoint(bezierPoint, Brushes.Green);
        }

        // Rajz egy vonalat
        private void DrawLine(Point p1, Point p2, Brush color)
        {
            Line line = new Line
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = color,
                StrokeThickness = 2
            };
            canvas.Children.Add(line);
        }

        // Rajz egy pontot
        private void DrawPoint(Point p, Brush color)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = color
            };
            Canvas.SetLeft(ellipse, p.X - 3);
            Canvas.SetTop(ellipse, p.Y - 3);
            canvas.Children.Add(ellipse);
        }

        // T paraméter csúszka frissítése
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            t = e.NewValue;
            DrawBezier();
        }

        // Kontrollpontok generálása a felhasználó által megadott szám alapján
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            int pointCount;
            if (int.TryParse(pointCountTextBox.Text, out pointCount) && pointCount >= 2)
            {
                controlPoints.Clear();
                double xSpacing = canvas.ActualWidth / (pointCount + 1);
                double yMid = canvas.ActualHeight / 2;

                // Kontrollpontok elhelyezése egyenletesen
                for (int i = 0; i < pointCount; i++)
                {
                    controlPoints.Add(new Point(xSpacing * (i + 1), yMid));
                }
                DrawBezier(); // Újratervezzük a Bezier görbét
            }
            else
            {
                MessageBox.Show("Kérlek, adj meg egy érvényes számot 2-nél nagyobbat!");
            }
        }
    }
}
