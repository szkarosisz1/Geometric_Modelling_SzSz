using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SurfaceVisualizer
{
    public partial class MainWindow : Window
    {
        private Model3DGroup modelGroup;
        private PerspectiveCamera camera;
        private Transform3DGroup transformGroup;
        private RotateTransform3D rotateX, rotateY;
        private ScaleTransform3D scaleTransform;

        private Point lastMousePosition;
        private double rotationX = 0;
        private double rotationY = 0;
        private double scale = 1.0;

        public MainWindow()
        {
            InitializeComponent();
            SetupViewport();
            DrawSurface();

            // Eseménykezelők az interakcióhoz
            viewport3D.MouseMove += Viewport3D_MouseMove;
            viewport3D.MouseWheel += Viewport3D_MouseWheel;
            viewport3D.MouseDown += Viewport3D_MouseDown;
        }

        private void SetupViewport()
        {
            modelGroup = new Model3DGroup();

            // Fények hozzáadása
            modelGroup.Children.Add(new AmbientLight(Colors.Gray));
            modelGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1)));

            // Kamera beállítása
            camera = new PerspectiveCamera
            {
                Position = new Point3D(0, 0, 10),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };

            viewport3D.Camera = camera;

            // Transformációk (forgatás és méretezés)
            transformGroup = new Transform3DGroup();
            rotateX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rotationX));
            rotateY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), rotationY));
            scaleTransform = new ScaleTransform3D(scale, scale, scale);

            transformGroup.Children.Add(rotateX);
            transformGroup.Children.Add(rotateY);
            transformGroup.Children.Add(scaleTransform);

            ModelVisual3D modelVisual = new ModelVisual3D { Content = modelGroup };
            viewport3D.Children.Add(modelVisual);
        }

        private void DrawSurface()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            int resolution = 20;

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    double u1 = i / (double)resolution * 2 * Math.PI;
                    double v1 = j / (double)resolution * 2 * Math.PI;
                    double u2 = (i + 1) / (double)resolution * 2 * Math.PI;
                    double v2 = (j + 1) / (double)resolution * 2 * Math.PI;

                    mesh.Positions.Add(EvaluateSurface(u1, v1));
                    mesh.Positions.Add(EvaluateSurface(u2, v1));
                    mesh.Positions.Add(EvaluateSurface(u1, v2));

                    mesh.Positions.Add(EvaluateSurface(u2, v1));
                    mesh.Positions.Add(EvaluateSurface(u2, v2));
                    mesh.Positions.Add(EvaluateSurface(u1, v2));
                }
            }

            GeometryModel3D surfaceModel = new GeometryModel3D
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.LightBlue)),
                Transform = transformGroup
            };

            modelGroup.Children.Add(surfaceModel);
        }

        private Point3D EvaluateSurface(double u, double v)
        {
            double x = Math.Cos(u) * (3 + Math.Cos(v));
            double y = Math.Sin(u) * (3 + Math.Cos(v));
            double z = Math.Sin(v);
            return new Point3D(x, y, z);
        }

        private void Viewport3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePosition = e.GetPosition(viewport3D);
        }

        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(viewport3D);
                double dx = currentPosition.X - lastMousePosition.X;
                double dy = currentPosition.Y - lastMousePosition.Y;

                rotationY += dx * 0.5;
                rotationX -= dy * 0.5;

                ((AxisAngleRotation3D)rotateX.Rotation).Angle = rotationX;
                ((AxisAngleRotation3D)rotateY.Rotation).Angle = rotationY;

                lastMousePosition = currentPosition;
            }
        }

        private void Viewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scale += e.Delta > 0 ? 0.1 : -0.1;
            scale = Math.Max(0.1, scale); // Minimális méret

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
            scaleTransform.ScaleZ = scale;
        }
    }
}
