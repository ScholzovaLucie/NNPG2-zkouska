using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private List<(Action<Graphics> Method, string Name)> drawingMethods;
        private int currentMethodIndex = 0;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Inicializace časovače
            timer = new Timer();
            timer.Interval = 2000; // 2 sekundy interval
            timer.Tick += Timer_Tick;

            // Inicializace seznamu metod pro kreslení
            drawingMethods = new List<(Action<Graphics> Method, string Name)>()
            {
                (Rotace, "Rotace"),
                (DrawSlunce, "DrawSlunce"),
                (DrawDashedLine, "DrawDashedLine"),
                (DrawSolidBrush, "DrawSolidBrush"),
                (DrawTextureBrush, "DrawTextureBrush"),
                (DrawHatchBrush, "DrawHatchBrush"),
                (DrawLinearGradientBrush, "DrawLinearGradientBrush"),
                (DrawPathGradientBrush, "DrawPathGradientBrush"),
                (ApplyTransformations, "ApplyTransformations"),
                (ApplyClipping, "ApplyClipping"),
                (DrawControlPaint, "DrawControlPaint"),
                (DrawImage, "DrawImage"),
                (DrawText, "DrawText"),
                (ApplyMatrix, "ApplyMatrix"),
                (DrawGraphicsPath, "DrawGraphicsPath"),
                (DrawTextPath, "DrawTextPath"),
                ((g) => BruteForceTriangulation(g, new List<Point>() { new Point(10, 10), new Point(50, 50), new Point(100, 10) }), "BruteForceTriangulation"),
                ((g) => {
                    List<Point> points = new List<Point>() { new Point(10, 10), new Point(50, 50), new Point(100, 10) };
                    List<Point> hull = ConvexHullGrahamScan(points);
                    DrawPolygon(g, hull);
                }, "ConvexHullGrahamScan"),
                ((g) => DrawHistogram(g, ResizeBitmap(Properties.Resources.Image, 100, 100)), "DrawHistogram"),
                ((g) => DrawBrightness(g, ResizeBitmap(Properties.Resources.Image, 100, 100)), "DrawBrightness"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.Image, 100, 100);
                    Bitmap result = Thinning(img);
                    g.DrawImage(result, new Point(0, 0));
                }, "Thinning"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.IKONA, 100, 100);
                    Bitmap result = AverageFilter(img);
                    g.DrawImage(result, new Point(0, 0));
                }, "AverageFilter"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.IKONA, 100, 100);
                    Bitmap result = GlobalThreshold(img, 128);
                    g.DrawImage(result, new Point(0, 0));
                }, "GlobalThreshold"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.IKONA, 100, 100);
                    Bitmap result = LocalThreshold(img, 3);
                    g.DrawImage(result, new Point(0, 0));
                }, "LocalThreshold"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.IKONA, 100, 100);
                    Bitmap result = SobelEdgeDetection(img);
                    g.DrawImage(result, new Point(0, 0));
                }, "SobelEdgeDetection"),
                ((g) => {
                    Bitmap img = ResizeBitmap(Properties.Resources.IKONA, 100, 100);
                    Bitmap result = Sharpen(img);
                    g.DrawImage(result, new Point(0, 0));
                }, "Sharpen"),
                (DrawResizedImage, "DrawResizedImage")
            };

            // Spustit časovač
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate(); // Vymazání plátna a vyvolání události Paint
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                if (currentMethodIndex < drawingMethods.Count)
                {
                    this.Text = drawingMethods[currentMethodIndex].Name;
                    drawingMethods[currentMethodIndex].Method(g);
                    currentMethodIndex++;
                }
                else
                {
                    currentMethodIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při vykreslování: {ex.Message}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Bitmap ResizeBitmap(Bitmap img, int width, int height)
        {
            Bitmap resizedImg = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImg))
            {
                g.DrawImage(img, new Rectangle(0, 0, width, height));
            }
            return resizedImg;
        }

        private void Rotace(Graphics g)
        {
            int size = 300;
            int mezera = 5;
            Bitmap bit = new Bitmap(size, size);
            using (Graphics g2 = Graphics.FromImage(bit))
            {
                Pen pen;
                int x = mezera;

                for (int y = 0; y < (size / mezera); y++)
                {
                    pen = (y % 2 == 0) ? new Pen(Color.Aqua) : new Pen(Color.BlueViolet);

                    Point start = new Point(x, 0);
                    Point end = new Point(x, size);
                    g2.DrawLine(pen, start, end);
                    x += mezera;
                }
            }

            TextureBrush textB = new TextureBrush(bit);
            textB.RotateTransform(20);
            g.FillRectangle(textB, 10, 10, 100, 100);
            g.DrawRectangle(new Pen(Color.Red), 10, 10, 100, 100);
        }

        private void DrawSlunce(Graphics g)
        {
            int polomer = 100;
            int prumer = polomer * 2;
            Point center = new Point(400, 300);
            g.DrawEllipse(new Pen(Color.Red), center.X - polomer, center.Y - polomer, prumer, prumer);

            g.DrawEllipse(new Pen(Color.Red), center.X - 2, center.Y - 2, 5, 5);

            float angle = 360f / 12;

            for (int i = 0; i < 12; i++)
            {
                g.Save();
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(i * angle);
                g.FillRectangle(new SolidBrush(Color.Black), -5, -polomer, 5, 20);
                g.ResetTransform();
            }

            angle = 360f / 60;

            for (int i = 0; i < 60; i++)
            {
                g.Save();
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(i * angle);
                g.FillRectangle(new SolidBrush(Color.Black), -5, -polomer, 2, 10);
                g.ResetTransform();
            }
        }

        public void DrawLine(Graphics g, Point start, Point end)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawLine(pen, start, end);
            }
        }

        public void DrawPolygon(Graphics g, List<Point> points)
        {
            if (points.Count > 1)
            {
                g.DrawPolygon(Pens.Black, points.ToArray());
            }
        }

        public void DrawDashedLine(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                pen.DashStyle = DashStyle.Dash;
                pen.DashOffset = 2;
                pen.DashPattern = new float[] { 4.0f, 2.0f };
                pen.DashCap = DashCap.Round;
                pen.LineJoin = LineJoin.Round;
                g.DrawLine(pen, 10, 10, 200, 10);
            }
        }

        public void DrawRectangle(Graphics g, Rectangle rect)
        {
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                g.FillRectangle(brush, rect);
            }
        }

        public void DrawLine(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawLine(pen, 10, 10, 200, 10);
            }
        }

        public void DrawCurve(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                Point[] points = { new Point(10, 20), new Point(20, 40), new Point(50, 30) };
                g.DrawCurve(pen, points);
            }
        }

        public void DrawBezier(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawBezier(pen, new Point(10, 50), new Point(50, 100), new Point(200, 100), new Point(300, 50));
            }
        }

        public void DrawColoredRectangle(Graphics g)
        {
            Color color = Color.FromArgb(255, 128, 0); // Oranžová barva
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, new Rectangle(10, 10, 100, 50));
            }
        }

        public void DrawSolidBrush(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                g.FillRectangle(brush, new Rectangle(10, 10, 100, 50));
            }
        }

        public void DrawTextureBrush(Graphics g)
        {
            Image image = Properties.Resources.texture;
            using (TextureBrush brush = new TextureBrush(image))
            {
                g.FillRectangle(brush, new Rectangle(120, 10, 100, 50));
            }
        }

        public void DrawHatchBrush(Graphics g)
        {
            using (HatchBrush brush = new HatchBrush(HatchStyle.Cross, Color.Red, Color.Yellow))
            {
                g.FillRectangle(brush, new Rectangle(230, 10, 100, 50));
            }
        }

        public void DrawLinearGradientBrush(Graphics g)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(10, 70, 100, 50), Color.Red, Color.Blue, 45f))
            {
                g.FillRectangle(brush, new Rectangle(10, 70, 100, 50));
            }
        }

        public void DrawPathGradientBrush(Graphics g)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(10, 130, 100, 50);
            using (PathGradientBrush brush = new PathGradientBrush(path))
            {
                brush.CenterColor = Color.White;
                brush.SurroundColors = new Color[] { Color.Black };
                g.FillPath(brush, path);
            }
        }

        public void ApplyTransformations(Graphics g)
        {
            g.TranslateTransform(50, 50);
            g.RotateTransform(45);
            g.ScaleTransform(1.5f, 1.5f);

            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, new Rectangle(10, 10, 100, 50));
            }

            g.ResetTransform();
        }

        public void ApplyClipping(Graphics g)
        {
            Region region = new Region(new Rectangle(50, 50, 100, 100));
            g.SetClip(region, CombineMode.Replace);

            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                g.FillRectangle(brush, new Rectangle(10, 10, 200, 200));
            }

            g.ResetClip();
        }

        public void DrawControlPaint(Graphics g)
        {
            Rectangle rect = new Rectangle(10, 10, 100, 50);
            ControlPaint.DrawButton(g, rect, ButtonState.Normal);
        }

        public void DrawImage(Graphics g)
        {
            Image image = Properties.Resources.Image;
            g.DrawImage(image, new Rectangle(10, 10, image.Width, image.Height));
        }

        public void DrawText(Graphics g)
        {
            using (Font font = new Font("Arial", 16))
            {
                g.DrawString("Hello, GDI+!", font, Brushes.Black, new PointF(10, 10));
            }
        }

        public void ApplyMatrix(Graphics g)
        {
            Matrix matrix = new Matrix();
            matrix.Translate(50, 50);
            matrix.Rotate(45);
            matrix.Scale(1.5f, 1.5f);
            g.Transform = matrix;

            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, new Rectangle(10, 10, 100, 50));
            }
        }

        public void DrawGraphicsPath(Graphics g)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(10, 10, 100, 50);
            path.AddRectangle(new Rectangle(120, 10, 100, 50));

            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawPath(pen, path);
            }
        }

        public void DrawTextPath(Graphics g)
        {
            DateTime now = DateTime.Now;
            string timeString = now.ToString("HH:mm:ss");
            Font font = new Font("Courier New", 10, FontStyle.Bold);
            Pen pen = new Pen(Color.Black, 2);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(timeString, font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), StringFormat.GenericDefault);
                g.DrawPath(pen, path);
                g.FillPath(Brushes.Transparent, path);
            }
        }

        public void PrintDocument()
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString("Hello, GDI+ Print!", new Font("Arial", 16), Brushes.Black, new PointF(100, 100));
        }

        public void BruteForceTriangulation(Graphics g, List<Point> points)
        {
            Pen pen = new Pen(Color.Black);

            for (int i = 0; i < points.Count - 2; i++)
            {
                for (int j = i + 1; j < points.Count - 1; j++)
                {
                    for (int k = j + 1; k < points.Count; k++)
                    {
                        g.DrawLine(pen, points[i], points[j]);
                        g.DrawLine(pen, points[j], points[k]);
                        g.DrawLine(pen, points[k], points[i]);
                    }
                }
            }
        }

        public List<Point> ConvexHullBruteForce(List<Point> points)
        {
            List<Point> hull = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) continue;

                    bool isEdge = true;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j) continue;

                        int crossProduct = (points[j].X - points[i].X) * (points[k].Y - points[i].Y) -
                                           (points[j].Y - points[i].Y) * (points[k].X - points[i].X);

                        if (crossProduct < 0)
                        {
                            isEdge = false;
                            break;
                        }
                    }

                    if (isEdge)
                    {
                        hull.Add(points[i]);
                        break;
                    }
                }
            }

            return hull;
        }

        public List<Point> ConvexHullGrahamScan(List<Point> points)
        {
            if (points.Count <= 3) return points;

            points.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            Point pivot = points[0];

            points = points.OrderBy(p => Math.Atan2(p.Y - pivot.Y, p.X - pivot.X)).ToList();

            Stack<Point> hull = new Stack<Point>();
            hull.Push(points[0]);
            hull.Push(points[1]);

            for (int i = 2; i < points.Count; i++)
            {
                Point top = hull.Pop();
                while (hull.Count > 0 && CrossProductLength(hull.Peek(), top, points[i]) <= 0)
                {
                    top = hull.Pop();
                }
                hull.Push(top);
                hull.Push(points[i]);
            }

            return hull.ToList();
        }

        private int CrossProductLength(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        public void DrawHistogram(Graphics g, Bitmap image)
        {
            int[] histogram = new int[256];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int intensity = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
                    histogram[intensity]++;
                }
            }

            int max = histogram.Max();

            for (int i = 0; i < histogram.Length; i++)
            {
                int barHeight = (int)((histogram[i] / (float)max) * 100);
                g.DrawLine(Pens.Black, new Point(i, 150), new Point(i, 150 - barHeight));
            }
        }

        public void DrawBrightness(Graphics g, Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int brightness = (int)(pixel.GetBrightness() * 255);
                    Color brightnessColor = Color.FromArgb(brightness, brightness, brightness);
                    g.FillRectangle(new SolidBrush(brightnessColor), new Rectangle(x, y, 1, 1));
                }
            }
        }

        // Průměrování pro odstranění šumu
        public Bitmap AverageFilter(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);
            int[,] mask = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int sum = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = image.GetPixel(x + i, y + j);
                            sum += pixel.R * mask[i + 1, j + 1];
                        }
                    }
                    int avg = sum / 9;
                    result.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            return result;
        }

        // Globální prahování
        public Bitmap GlobalThreshold(Bitmap image, int threshold)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (pixel.R + pixel.G + pixel.B) / 3;
                    if (gray < threshold)
                        result.SetPixel(x, y, Color.Black);
                    else
                        result.SetPixel(x, y, Color.White);
                }
            }
            return result;
        }

        // Lokální prahování (průměrná hodnota)
        public Bitmap LocalThreshold(Bitmap image, int maskSize)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int sum = 0;
                    int count = 0;

                    for (int i = -maskSize / 2; i <= maskSize / 2; i++)
                    {
                        for (int j = -maskSize / 2; j <= maskSize / 2; j++)
                        {
                            int nx = x + i;
                            int ny = y + j;
                            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                            {
                                Color pixel = image.GetPixel(nx, ny);
                                sum += (pixel.R + pixel.G + pixel.B) / 3;
                                count++;
                            }
                        }
                    }

                    int average = sum / count;
                    Color originalPixel = image.GetPixel(x, y);
                    int gray = (originalPixel.R + originalPixel.G + originalPixel.B) / 3;

                    if (gray < average)
                        result.SetPixel(x, y, Color.Black);
                    else
                        result.SetPixel(x, y, Color.White);
                }
            }
            return result;
        }

        // Detekce hran (Sobelův operátor)
        public Bitmap SobelEdgeDetection(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);

            int[,] gx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int sumX = 0;
                    int sumY = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = image.GetPixel(x + i, y + j);
                            sumX += pixel.R * gx[i + 1, j + 1];
                            sumY += pixel.R * gy[i + 1, j + 1];
                        }
                    }
                    int magnitude = (int)Math.Sqrt(sumX * sumX + sumY * sumY);
                    magnitude = Math.Min(255, Math.Max(0, magnitude));
                    result.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }
            return result;
        }

        // Ostření obrazu
        public Bitmap Sharpen(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);

            int[,] laplacian = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int sum = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = image.GetPixel(x + i, y + j);
                            sum += pixel.R * laplacian[i + 1, j + 1];
                        }
                    }
                    sum = Math.Min(255, Math.Max(0, sum));
                    result.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }
            }
            return result;
        }

        // Hledání kostry(Skeletonization)
        public Bitmap Thinning(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap result = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result.SetPixel(x, y, image.GetPixel(x, y));
                }
            }

            bool hasChanged;
            do
            {
                hasChanged = false;
                bool[,] marker = new bool[width, height];

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        if (result.GetPixel(x, y).R == 0)
                        {
                            int A = 0;
                            int B = 0;
                            int[,] neighbors = new int[3, 3];

                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (result.GetPixel(x + i, y + j).R == 0)
                                        neighbors[i + 1, j + 1] = 1;
                                    else
                                        neighbors[i + 1, j + 1] = 0;
                                }
                            }

                            A = (neighbors[0, 1] == 0 && neighbors[0, 2] == 1 ? 1 : 0) +
                                (neighbors[0, 2] == 0 && neighbors[1, 2] == 1 ? 1 : 0) +
                                (neighbors[1, 2] == 0 && neighbors[2, 2] == 1 ? 1 : 0) +
                                (neighbors[2, 2] == 0 && neighbors[2, 1] == 1 ? 1 : 0) +
                                (neighbors[2, 1] == 0 && neighbors[2, 0] == 1 ? 1 : 0) +
                                (neighbors[2, 0] == 0 && neighbors[1, 0] == 1 ? 1 : 0) +
                                (neighbors[1, 0] == 0 && neighbors[0, 0] == 1 ? 1 : 0) +
                                (neighbors[0, 0] == 0 && neighbors[0, 1] == 1 ? 1 : 0);

                            B = neighbors[0, 1] + neighbors[0, 2] + neighbors[1, 2] + neighbors[2, 2] +
                                neighbors[2, 1] + neighbors[2, 0] + neighbors[1, 0] + neighbors[0, 0];

                            if (A == 1 && (B >= 2 && B <= 6) &&
                                (neighbors[0, 1] * neighbors[1, 2] * neighbors[2, 1] == 0) &&
                                (neighbors[0, 2] * neighbors[1, 2] * neighbors[1, 0] == 0))
                            {
                                marker[x, y] = true;
                                hasChanged = true;
                            }
                        }
                    }
                }

                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        if (marker[x, y])
                        {
                            result.SetPixel(x, y, Color.White);
                        }
                    }
                }
            } while (hasChanged);

            return result;
        }

        private void DrawResizedImage(Graphics g)
        {
            Bitmap img = Properties.Resources.IKONA;
            Bitmap resizedImg = ResizeBitmap(img, 100, 100);
            g.DrawImage(resizedImg, new Point(10, 10));
        }
    }
}
